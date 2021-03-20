#ifndef NOSIEINC
  #define NOISEINC
  ///ret : 0.0 - <1.0
  float rand(float n)
  {
    return frac(sin(n) * 43758.5453123);
  }

  ///ret : 0.0 - <1.0
  float rand(float2 co)
  {
    return rand(dot(co.xy, float2(12.9898, 78.233)));
  }

  ///ret : 0.0 - <1.0
  float rand(float3 vec)
  {
    //zは独自
    return rand(dot(vec, float3(12.9898, 78.233, 47.3562985)));
  }

  //ret : (0, 0) - <(1, 1)
  //3Dランダム
  float3 rand3D(float n)
  {
    float rand1 = rand(n);
    float rand2 = rand(rand1);
    float rand3 = rand(rand1 + n);
    float3 rand = float3(rand1, rand2, rand3);
    return - 1.0 + 2.0 * frac(sin(rand) * 43758.5453123);
  }
  float3 rand3D(float3 vec)
  {
    
    float rand1 = dot(vec, float3(4127.1, 3311.7, 2264.7));
    float rand2 = dot(vec, float3(2269.5, 5183.3, 1336.2));
    float rand3 = dot(vec, float3(1301.7, 8231.1, 5142.6));
    float3 rand = float3(rand1, rand2, rand3);
    return - 1.0 + 2.0 * frac(sin(rand) * 43758.5453123);
  }

  float3 rand3D(float2 vec)
  {
    
    float rand1 = dot(vec, float2(127.1, 311.7));
    float rand2 = dot(vec, float2(269.5, 183.3));
    float rand3 = dot(vec, float2(301.7, 231.1));
    
    float3 rand = float3(rand1, rand2, rand3);
    return - 1.0 + 2.0 * frac(sin(rand) * 43758.5453123);
  }


  ///ret : -1 - +1
  float2 rand2D(float2 st)
  {
    st = float2(dot(st, float2(127.1, 311.7)),
    dot(st, float2(269.5, 183.3)));
    return - 1.0 + 2.0 * frac(sin(st) * 43758.5453123);
  }
  //Perline Noise

  float pNoise(float pos)
  {
    return pNoise(float2(pos, 0));
  }

  float pNoise(float2 pos)
  {
    float2 i_o = floor(pos);
    float2 f = frac(pos);
    
    float2 sm = smoothstep(0, 1, f);

    float dot_o = 0;
    float dot_x = 0;
    float dot_y = 0;
    float dot_xy = 0;
    {
      float2 i_x = i_o + float2(1, 0);
      float2 i_y = i_o + float2(0, 1);
      float2 i_xy = i_o + float2(1, 1);
      float2 rand_o = rand2D(i_o);
      float2 rand_x = rand2D(i_x);
      float2 rand_y = rand2D(i_y);
      float2 rand_xy = rand2D(i_xy);
      
      float2 toPos_o = pos - i_o;
      float2 toPos_x = pos - i_x;
      float2 toPos_y = pos - i_y;
      float2 toPos_xy = pos - i_xy;

      dot_o = dot(rand_o, toPos_o) * 0.5 + 0.5;
      dot_x = dot(rand_x, toPos_x) * 0.5 + 0.5;
      dot_y = dot(rand_y, toPos_y) * 0.5 + 0.5;
      dot_xy = dot(rand_xy, toPos_xy) * 0.5 + 0.5;
    }
    
    float value1 = lerp(dot_o, dot_x, sm.x);
    float value2 = lerp(dot_y, dot_xy, sm.x);
    float value3 = lerp(0, value2 - value1, sm.y);
    return value1 + value3;
  }

  float pNoise(float3 pos)
  {
    float3 i_o = floor(pos);
    float3 f = frac(pos);
    
    float3 sm = smoothstep(0, 1, f);

    float dot_o = 0;
    float dot_x = 0;
    float dot_y = 0;
    float dot_z = 0;
    float dot_xy = 0;
    float dot_xz = 0;
    float dot_yz = 0;
    float dot_xyz = 0;
    {
      float3 i_x = i_o + float3(1, 0, 0);
      float3 i_y = i_o + float3(0, 1, 0);
      float3 i_z = i_o + float3(0, 0, 1);
      float3 i_xy = i_o + float3(1, 1, 0);
      float3 i_xz = i_o + float3(1, 0, 1);
      float3 i_yz = i_o + float3(0, 1, 1);
      float3 i_xyz = i_o + float3(1, 1, 1);
      float3 rand_o = rand3D(i_o);
      float3 rand_x = rand3D(i_x);
      float3 rand_y = rand3D(i_y);
      float3 rand_z = rand3D(i_z);
      float3 rand_xy = rand3D(i_xy);
      float3 rand_xz = rand3D(i_xz);
      float3 rand_yz = rand3D(i_yz);
      float3 rand_xyz = rand3D(i_xyz);
      
      float3 toPos_o = pos - i_o;
      float3 toPos_x = pos - i_x;
      float3 toPos_y = pos - i_y;
      float3 toPos_z = pos - i_z;
      float3 toPos_xy = pos - i_xy;
      float3 toPos_xz = pos - i_xz;
      float3 toPos_yz = pos - i_yz;
      float3 toPos_xyz = pos - i_xyz;
      
      dot_o = dot(rand_o, toPos_o) * 0.5 + 0.5;
      dot_x = dot(rand_x, toPos_x) * 0.5 + 0.5;
      dot_y = dot(rand_y, toPos_y) * 0.5 + 0.5;
      dot_z = dot(rand_z, toPos_z) * 0.5 + 0.5;
      dot_xy = dot(rand_xy, toPos_xy) * 0.5 + 0.5;
      dot_xz = dot(rand_xz, toPos_xz) * 0.5 + 0.5;
      dot_yz = dot(rand_yz, toPos_yz) * 0.5 + 0.5;
      dot_xyz = dot(rand_xyz, toPos_xyz) * 0.5 + 0.5;
    }

    //底面
    float value_x0 = lerp(dot_o, dot_x, sm.x);
    float value_x1 = lerp(dot_z, dot_xz, sm.x);
    float noiseXZ0 = lerp(value_x0, value_x1, sm.z);
    
    //天井
    float value_x2 = lerp(dot_y, dot_xy, sm.x);
    float value_x3 = lerp(dot_yz, dot_xyz, sm.x);
    float noiseXZ1 = lerp(value_x2, value_x3, sm.z);
    
    return lerp(noiseXZ0, noiseXZ1, sm.y);
  }

  float2 pNoise2D(float2 pos)
  {
    float2 pos_0 = pos;
    float2 pos_1 = float2(pos.y, pos.x);//対称性が生まれそうだから工夫が必要かも
    float n_0 = pNoise(pos_0);
    float n_1 = pNoise(pos_1);
    return float2(n_0, n_1);
  }

  float3 pNoise3D(float3 pos)
  {
    float3 pos_0 = pos;
    float3 pos_1 = float3(pos.y, pos.z, pos.x);
    float3 pos_2 = float3(pos.z, pos.x, pos.y);
    float n_0 = pNoise(pos_0);
    float n_1 = pNoise(pos_1);
    float n_2 = pNoise(pos_2);
    return float3(n_0, n_1, n_2);
  }

  //Cell
  float3 getNearCellPos(float3 pos)
  {
    float3 i_o = floor(pos);
    
    float nearDist = 100000000;
    float3 nearPos = 0;
    
    for (int i = -1; i <= 1; i ++)
    {
      
      for (int j = -1; j <= 1; j ++)
      {
        
        for (int k = -1; k <= 1; k ++)
        {
          float3 base_pos = i_o + float3(i, j, k);
          float3 rand_pos = base_pos + rand3D(base_pos) * 0.5 + 0.5;
          float dist = length(rand_pos - pos);
          if (dist < nearDist)
          {
            nearDist = dist;
            nearPos = rand_pos;
          }
        }
      }
    }
    
    return nearPos;
  }

  float2 getNearCellPos(float2 pos)
  {
    float2 i_o = floor(pos);
    
    float nearDist = 100000000;
    float2 nearPos = 0;
    
    for (int i = -1; i <= 1; i ++)
    {
      
      for (int j = -1; j <= 1; j ++)
      {
        float2 base_pos = i_o + float2(i, j);
        float2 rand_pos = base_pos + rand2D(base_pos) * 0.5 + 0.5;
        float dist = length(rand_pos - pos);
        if(dist < nearDist)
        {
          nearDist = dist;
          nearPos = rand_pos;
        }
      }
    }
    
    return nearPos;
  }

  ///Curl Noise

  float2 curlNoise(float2 pos)
  {
    const float epsilon = 0.00001;
    
    float2 n_px = pNoise2D(pos + float2(epsilon, 0));
    float2 n_mx = pNoise2D(pos - float2(epsilon, 0));
    float2 n_py = pNoise2D(pos + float2(0, epsilon));
    float2 n_my = pNoise2D(pos - float2(0, epsilon));

    float x = n_my.y - n_py.y;
    float y = n_px.x - n_mx.x;
    
    return normalize(float2(x, y));
  }

  float3 curlNoise(float3 pos)
  {
    const float epsilon = 0.00001;
    
    float3 n_px = pNoise3D(pos + float3(epsilon, 0, 0));
    float3 n_mx = pNoise3D(pos - float3(epsilon, 0, 0));
    float3 n_py = pNoise3D(pos + float3(0, epsilon, 0));
    float3 n_my = pNoise3D(pos - float3(0, epsilon, 0));
    float3 n_pz = pNoise3D(pos + float3(0, 0, epsilon));
    float3 n_mz = pNoise3D(pos - float3(0, 0, epsilon));

    float x = n_my.z - n_py.z - n_mz.y + n_pz.y;
    float y = n_px.z - n_mx.z - n_pz.x + n_mz.x;
    float z = n_mx.y - n_px.y - n_my.x + n_py.x;
    
    return normalize(float3(x, y, z) / 4.0);
  }
#endif