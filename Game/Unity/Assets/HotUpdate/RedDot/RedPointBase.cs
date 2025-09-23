

using System.Collections.Generic;
using UnityEngine.Events;

public class RedPointBase {
    public class Cell {
        public long Target;
        public UnityAction<bool> Callback;
        public RedDotManager.UpdateProps UpdatePropsFunc;
    }

    protected int _id;
    public int ID => _id;

    protected RedPointBase _parent;
    public RedPointBase Parent => _parent;

    protected List<RedPointBase> _children = new();
    public List<RedPointBase> Children => _children;

    protected Dictionary<long, Cell> _cells = new();

    public void Init(int id, RedPointBase parent) {
        _id = id;
        _parent = parent;

        GenerateCells();
    }

    public virtual void GenerateCells() {
        _cells.Add(0, new());
    }

    public void AddChild(RedPointBase child) {
        _children.Add(child);
    }

    public Cell AddCell(long target) {
        if (!_cells.TryGetValue(target, out Cell cell)) {
            cell = new();
            _cells.Add(target, cell);
        }
        cell.Target = target;
        return cell;
    }

    public void AddCallback2Cell(long target, UnityAction<bool> callback) {
        Cell cell = AddCell(target);
        cell.Callback = callback;
    }

    public void AddUpdateFunc2Cell(long target, RedDotManager.UpdateProps func) {
        Cell cell = AddCell(target);
        cell.UpdatePropsFunc = func;
    }

    public void RemoveCell(long target) {
        if (_cells.TryGetValue(target, out Cell cell)) {
            cell.Callback = null;
        }
    }

    protected virtual bool _isVisible(long target) => false;
    public bool IsVisible(long target) {
        // 自身是否需要显示红点
        if (_isVisible(target)) {
            return true;
        }

        // 子节点中是否有需要显示红点
        for (int i = 0; i < _children.Count; ++i) {
            RedPointBase child = _children[i];
            if (child == null) {
                continue;
            }

            if (child.IsVisible()) {
                return true;
            }
        }

        return false;
    }

    public bool IsVisible() {
        foreach ((long target, Cell cell) in _cells) {
            bool visible = IsVisible(target);
            if (visible) {
                return true;
            }
        }

        return false;
    }

    // 自下而上更新红点显示
    public void UpdateBottom2Top() {
        foreach ((long target, Cell cell) in _cells) {
            cell.Callback?.Invoke(IsVisible(target));
        }

        _parent?.UpdateBottom2Top();
    }

    // 自上而下更新红点显示
    public void UpdateTop2Bottom() {
        foreach ((long target, Cell cell) in _cells) {
            cell.Callback?.Invoke(IsVisible(target));
        }

        foreach (RedPointBase child in _children) {
            child.UpdateTop2Bottom();
        }
    }
}

public class RedPointBaseProps {
}

public class RedPointBase<T>: RedPointBase where T : RedPointBaseProps {

    protected T UpdateProps(long target) {
        T props = null;
        if (_cells.TryGetValue(target, out Cell cell)) {
            props = cell.UpdatePropsFunc?.Invoke() as T;
        }
        return props;
    }
}
