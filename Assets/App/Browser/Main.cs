using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Video;
using UnityEngine;


enum BrowseItem {
    Device,
    Service,
    Item
}

public class Main : MonoBehaviour
{    
    public GameObject target = null;
    public VideoPlayer videoPlayer = null;
#if UNITY_ANDROID
    AndroidJavaObject dlnaClient = null;
    BrowseItem state = BrowseItem.Device;

    bool needsUpdate = false;

    // For Device
    List<ScrollableList.ListItem> devices = new List<ScrollableList.ListItem>();

    // For Service
    AndroidJavaObject activeDevice = null;
    List<ScrollableList.ListItem> services = new List<ScrollableList.ListItem>();

    // For Item
    class ItemData {
        public AndroidJavaObject item = null;
        public AndroidJavaObject container = null;
    }
    Stack<string> paths = new Stack<string>();
    AndroidJavaObject activeService = null;
    List<ScrollableList.ListItem> items = new List<ScrollableList.ListItem>();

    class DeviceListener: AndroidJavaProxy {
        Main parent = null;
        public DeviceListener(Main parent): base("jp.mzp.needleplayer.dlnaclient.DeviceListener") {
            this.parent = parent;
        }

        void remoteDeviceAdded(AndroidJavaObject device) {
            parent.AddDevice(device);
        }

        void remoteDeviceUpdated(AndroidJavaObject device) {            
            parent.UpdateDevice(device);
        }

        void remoteDeviceRemoved(AndroidJavaObject device) {        
        }
    }

    class BrowseCallback: AndroidJavaProxy {
        Main parent = null;
        public BrowseCallback(Main parent): base("jp.mzp.needleplayer.dlnaclient.BrowseCallback") {
            this.parent = parent;
        }

        public void receive(AndroidJavaObject content) {
            parent.EnterItem(content);
        }
    }

    AndroidJavaObject getActivity()
    {
       var unityPackageName = "com.unity3d.player.UnityPlayer";
       var unityActivityName = "currentActivity";
       var activityClass = new AndroidJavaClass(unityPackageName);
       return activityClass.GetStatic<AndroidJavaObject>(unityActivityName);
    }

    void StartListening() 
    {   
        var activity = getActivity();

        var packageName = "jp.mzp.needleplayer.dlnaclient.DLNAClient";
    
        // 通常メソッド
        this.dlnaClient = new AndroidJavaObject(packageName, activity);
        dlnaClient.Call("addDeviceListener", new DeviceListener(this));
    }

    public void OnClick(ScrollableList.ListItem item) {
        switch(state) {
            case BrowseItem.Device:
                activeDevice = (AndroidJavaObject)item.data;
                services = activeDevice.Call<AndroidJavaObject[]>("getServices").Select((service) => {
                    AndroidJavaObject serviceType = service.Call<AndroidJavaObject>("getServiceType");
                    string type = serviceType.Call<string>("getType");
                    return new ScrollableList.ListItem(type, service);
                }).ToList();
                state = BrowseItem.Service;
                break;
            case BrowseItem.Service:
                {
                    string path = "0";
                    activeService = (AndroidJavaObject)item.data;
                    dlnaClient.Call("browse", activeService, path, new BrowseCallback(this));
                    paths.Push(path);
                }
                break;
            case BrowseItem.Item:
                ItemData data = (ItemData)item.data;

                if (data.container != null) {
                    // container
                    string path = data.container.Call<string>("getId");
                    dlnaClient.Call("browse", activeService, path, new BrowseCallback(this));
                    paths.Push(path);
                } else {
                    // item
                    string url = data.item.Call<AndroidJavaObject>("getFirstResource").Call<string>("getValue");
                    videoPlayer.url = url;
                }
                break;
        }
        Reload();
    }

    public void OnBack() {
        switch (state) {
            case BrowseItem.Device:
                // do nothing
                break;
            case BrowseItem.Service:
                state = BrowseItem.Device;
                break;
            case BrowseItem.Item:
                if (paths.Count() == 0) {
                    state = BrowseItem.Service;
                } else {
                    paths.Pop();
                    dlnaClient.Call("browse", activeService, paths.Peek(), new BrowseCallback(this));
                }
                break;
            default:
                break;
        }
        Reload();
    }

    void AddDevice(AndroidJavaObject device) {
        if(!IsDLNA(device)) { return; }
        string title = device.Call<string>("getDisplayString");
        ScrollableList.ListItem item = new ScrollableList.ListItem(title, device);
        devices.Add(item);
        needsUpdate = true;
    }

    void UpdateDevice(AndroidJavaObject device) {
        if(!IsDLNA(device)) { return; }
        string title = device.Call<string>("getDisplayString");
        for(int i = 0; i < devices.Count; i++) {
            if (devices[i].title == title) {
                devices[i].data = device;
                needsUpdate = true;
                break;
            }
        }
    }

    bool IsDLNA(AndroidJavaObject device) {
        foreach(var service in device.Call<AndroidJavaObject[]>("getServices")) {
            AndroidJavaObject serviceType = service.Call<AndroidJavaObject>("getServiceType");
            string type = serviceType.Call<string>("getType");
            if(type.Contains("Content")) {
                return true;
            }
        }
        return false;
    }

    void EnterItem(AndroidJavaObject content) {
        items.Clear();
        foreach (var container in dlnaClient.CallStatic<AndroidJavaObject[]>("getContainers", content)) {
            ItemData data = new ItemData();
            data.container = container;
            items.Add(new ScrollableList.ListItem(container.Call<string>("getTitle"), data));
        }
        foreach (var item in dlnaClient.CallStatic<AndroidJavaObject[]>("getItems", content)) {
            ItemData data = new ItemData();
            data.item = item;
            items.Add(new ScrollableList.ListItem(item.Call<string>("getTitle"), data));
        }
        state = BrowseItem.Item;
        Reload();
    }

    void Reload() {
        switch (state) {
        case BrowseItem.Device:
            scrollableList.items = devices;
            break;
        case BrowseItem.Service:
            scrollableList.items = services;
            break;
        case BrowseItem.Item:
            scrollableList.items = items;
            break;
        }   
    }

    bool ContainsDevice(string deviceName) {
        foreach (var device in devices) {
            if (device.title == deviceName) {
                return true;
            }
        }
        return false;
    }

    ScrollableList scrollableList {
        get => target.GetComponent<ScrollableList>();
    } 

    // Start is called before the first frame update
    void Start()
    {
        scrollableList.onClick = OnClick;
        StartListening();
    }

    void Update() {
        if (needsUpdate) {
            Reload();
            needsUpdate = false;
        }
    }
#endif
}
