

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class RedDotManager : Singleton<RedDotManager> {
    public delegate RedPointBaseProps UpdateProps();

    private Dictionary<int, RedPointBase> _points = new();
    
    public class RedPointConfig {
        public int Id;
        public int ParentID;
        public int Order;
    }

    public void Initialize() {
        Debug.Log("红点初始化");
        // List<RedPointConfig> configs = RedPointConfigCategory.Instance.GetAll().Values.ToList();
        List<RedPointConfig> configs = new();
        configs.Sort((a, b) => a.Order - b.Order);
        for (int i = 0; i < configs.Count; ++i) {
            RedPointConfig config = configs[i];
            RedPointBase parent = GetPoint(config.ParentID);

            Type component = null;
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var comp in types) {
                var attribute = comp.GetCustomAttribute<RedPointAttribute>();
                if (attribute != null && attribute.ID == config.Id) {
                    component = comp;
                    break;
                }
            }

            RedPointBase point;
            if (component != null) {
                point = Activator.CreateInstance(component) as RedPointBase;
            }
            else {
                point = new();
            }
            point?.Init(config.Id, parent);

            parent?.AddChild(point);
            _points.Add(config.Id, point);
        }
    }

    public RedPointBase GetPoint(int id) {
        if (_points.TryGetValue(id, out RedPointBase point)) {
            return point;
        }

        return null;
    }

    public void Bind(int id, long target, UnityAction<bool> callback) {
        if (!_points.TryGetValue(id, out RedPointBase point)) {
            Debug.LogError($"红点不存在 => {id}");
            return;
        }

        point.AddCallback2Cell(target, callback);

        point.UpdateBottom2Top();
    }

    public void Unbind(int id, long target) {
        if (!_points.TryGetValue(id, out RedPointBase point)) {
            Debug.LogError($"红点不存在 => {id}");
            return;
        }
        point.RemoveCell(target);
    }

    // 自下而上更新红点显示
    public void UpdateBottom2Top(int id) {
        RedPointBase point = GetPoint(id);
        point?.UpdateBottom2Top();
    }

    // 自上而下更新红点显示
    public void UpdateTop2Bottom() {
        RedPointBase point = GetPoint(1);
        point.UpdateTop2Bottom();
    }
}
