using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class NodeModel {
    [RealtimeProperty(1, true, true)] private int _nodeIndex;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class NodeModel : RealtimeModel {
    public int nodeIndex {
        get {
            return _cache.LookForValueInCache(_nodeIndex, entry => entry.nodeIndexSet, entry => entry.nodeIndex);
        }
        set {
            if (this.nodeIndex == value) return;
            _cache.UpdateLocalCache(entry => { entry.nodeIndexSet = true; entry.nodeIndex = value; return entry; });
            InvalidateReliableLength();
            FireNodeIndexDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(NodeModel model, T value);
    public event PropertyChangedHandler<int> nodeIndexDidChange;
    
    private struct LocalCacheEntry {
        public bool nodeIndexSet;
        public int nodeIndex;
    }
    
    private LocalChangeCache<LocalCacheEntry> _cache = new LocalChangeCache<LocalCacheEntry>();
    
    public enum PropertyID : uint {
        NodeIndex = 1,
    }
    
    public NodeModel() : this(null) {
    }
    
    public NodeModel(RealtimeModel parent) : base(null, parent) {
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        UnsubscribeClearCacheCallback();
    }
    
    private void FireNodeIndexDidChange(int value) {
        try {
            nodeIndexDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        int length = 0;
        if (context.fullModel) {
            FlattenCache();
            length += WriteStream.WriteVarint32Length((uint)PropertyID.NodeIndex, (uint)_nodeIndex);
        } else if (context.reliableChannel) {
            LocalCacheEntry entry = _cache.localCache;
            if (entry.nodeIndexSet) {
                length += WriteStream.WriteVarint32Length((uint)PropertyID.NodeIndex, (uint)entry.nodeIndex);
            }
        }
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var didWriteProperties = false;
        
        if (context.fullModel) {
            stream.WriteVarint32((uint)PropertyID.NodeIndex, (uint)_nodeIndex);
        } else if (context.reliableChannel) {
            LocalCacheEntry entry = _cache.localCache;
            if (entry.nodeIndexSet) {
                _cache.PushLocalCacheToInflight(context.updateID);
                ClearCacheOnStreamCallback(context);
            }
            if (entry.nodeIndexSet) {
                stream.WriteVarint32((uint)PropertyID.NodeIndex, (uint)entry.nodeIndex);
                didWriteProperties = true;
            }
            
            if (didWriteProperties) InvalidateReliableLength();
        }
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            switch (propertyID) {
                case (uint)PropertyID.NodeIndex: {
                    int previousValue = _nodeIndex;
                    _nodeIndex = (int)stream.ReadVarint32();
                    bool nodeIndexExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.nodeIndexSet);
                    if (!nodeIndexExistsInChangeCache && _nodeIndex != previousValue) {
                        FireNodeIndexDidChange(_nodeIndex);
                    }
                    break;
                }
                default: {
                    stream.SkipProperty();
                    break;
                }
            }
        }
    }
    
    #region Cache Operations
    
    private StreamEventDispatcher _streamEventDispatcher;
    
    private void FlattenCache() {
        _nodeIndex = nodeIndex;
        _cache.Clear();
    }
    
    private void ClearCache(uint updateID) {
        _cache.RemoveUpdateFromInflight(updateID);
    }
    
    private void ClearCacheOnStreamCallback(StreamContext context) {
        if (_streamEventDispatcher != context.dispatcher) {
            UnsubscribeClearCacheCallback(); // unsub from previous dispatcher
        }
        _streamEventDispatcher = context.dispatcher;
        _streamEventDispatcher.AddStreamCallback(context.updateID, ClearCache);
    }
    
    private void UnsubscribeClearCacheCallback() {
        if (_streamEventDispatcher != null) {
            _streamEventDispatcher.RemoveStreamCallback(ClearCache);
            _streamEventDispatcher = null;
        }
    }
    
    #endregion
}
/* ----- End Normal Autogenerated Code ----- */
