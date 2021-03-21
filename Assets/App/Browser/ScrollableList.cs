using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollableList : MonoBehaviour
{
    public class ListItem {
        public string title;
        public object data;

        public ListItem(string title, object data) {
            this.title = title;
            this.data = data;
        }
    }

    [SerializeField]
    private Transform SpawnPoint = null;

    [SerializeField]
    private GameObject Item = null;

    [SerializeField]
    private RectTransform content = null;

    private List<ListItem> _items = new List<ListItem>();
    public List<ListItem> items {
        get => _items;
        set {
            _items = value;
            Reload();
        }
    }

    public Action<ListItem> onClick = null;

    void Reload() {
        // Remove all children object
        foreach (Transform child in SpawnPoint.transform) {
           GameObject.Destroy(child.gameObject);
        }

        RectTransform rectTransform = Item.GetComponent<RectTransform>();
        float spawnY = 0;

        for (int i = 0; i < items.Count; i++) {
            ListItem item = items[i];
            spawnY += rectTransform.rect.height;
            Vector3 pos = new Vector3(0, -spawnY, SpawnPoint.position.z);

            GameObject SpawnedItem = Instantiate(Item, pos, SpawnPoint.rotation);
            SpawnedItem.transform.SetParent(SpawnPoint, false);
            ItemDetail itemDetail = SpawnedItem.GetComponent<ItemDetail>();
            itemDetail.text.text = item.title;

            EventTrigger trigger = SpawnedItem.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((_) => { 
                if (onClick != null) {
                    onClick(item);
                }
            });
            trigger.triggers.Add(entry);
        }

        content.sizeDelta = new Vector2(0, spawnY);
    }
}
