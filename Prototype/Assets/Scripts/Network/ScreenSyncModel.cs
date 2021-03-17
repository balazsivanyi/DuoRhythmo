using System;
using Normal.Realtime.Serialization;
using UnityEngine;
using Normal.Realtime;

[RealtimeModel]
public partial class ScreenSyncModel {
    [RealtimeProperty(1, true, true)] private RealtimeArray<NodeModel> _nodesArray;
    [RealtimeProperty(2, true, true)] private int _numberOfNodes;
    [RealtimeProperty(3, true, true)] private int _bpm;
    // [RealtimeProperty(1, true, true)] private bool _isDrum;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class ScreenSyncModel : RealtimeModel {
    public int numberOfNodes {
        get {
            return _cache.LookForValueInCache(_numberOfNodes, entry => entry.numberOfNodesSet, entry => entry.numberOfNodes);
        }
        set {
            if (this.numberOfNodes == value) return;
            _cache.UpdateLocalCache(entry => { entry.numberOfNodesSet = true; entry.numberOfNodes = value; return entry; });
            InvalidateReliableLength();
            FireNumberOfNodesDidChange(value);
        }
    }
    
    public int bpm {
        get {
            return _cache.LookForValueInCache(_bpm, entry => entry.bpmSet, entry => entry.bpm);
        }
        set {
            if (this.bpm == value) return;
            _cache.UpdateLocalCache(entry => { entry.bpmSet = true; entry.bpm = value; return entry; });
            InvalidateReliableLength();
            FireBpmDidChange(value);
        }
    }
    
    public Normal.Realtime.Serialization.RealtimeArray<NodeModel> nodesArray {
        get { return _nodesArray; }
    }
    
    public delegate void PropertyChangedHandler<in T>(ScreenSyncModel model, T value);
    public event PropertyChangedHandler<int> numberOfNodesDidChange;
    public event PropertyChangedHandler<int> bpmDidChange;
    
    private struct LocalCacheEntry {
        public bool numberOfNodesSet;
        public int numberOfNodes;
        public bool bpmSet;
        public int bpm;
    }
    
    private LocalChangeCache<LocalCacheEntry> _cache = new LocalChangeCache<LocalCacheEntry>();
    
    public enum PropertyID : uint {
        NodesArray = 1,
        NumberOfNodes = 2,
        Bpm = 3,
    }
    
    public ScreenSyncModel() : this(null) {
    }
    
    public ScreenSyncModel(RealtimeModel parent) : base(null, parent) {
        RealtimeModel[] childModels = new RealtimeModel[1];
        
        _nodesArray = new Normal.Realtime.Serialization.RealtimeArray<NodeModel>();
        childModels[0] = _nodesArray;
        
        SetChildren(childModels);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        UnsubscribeClearCacheCallback();
    }
    
    private void FireNumberOfNodesDidChange(int value) {
        try {
            numberOfNodesDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireBpmDidChange(int value) {
        try {
            bpmDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        int length = 0;
        if (context.fullModel) {
            FlattenCache();
            length += WriteStream.WriteVarint32Length((uint)PropertyID.NumberOfNodes, (uint)_numberOfNodes);
            length += WriteStream.WriteVarint32Length((uint)PropertyID.Bpm, (uint)_bpm);
        } else if (context.reliableChannel) {
            LocalCacheEntry entry = _cache.localCache;
            if (entry.numberOfNodesSet) {
                length += WriteStream.WriteVarint32Length((uint)PropertyID.NumberOfNodes, (uint)entry.numberOfNodes);
            }
            if (entry.bpmSet) {
                length += WriteStream.WriteVarint32Length((uint)PropertyID.Bpm, (uint)entry.bpm);
            }
        }
        length += WriteStream.WriteCollectionLength((uint)PropertyID.NodesArray, _nodesArray, context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var didWriteProperties = false;
        
        if (context.fullModel) {
            stream.WriteVarint32((uint)PropertyID.NumberOfNodes, (uint)_numberOfNodes);
            stream.WriteVarint32((uint)PropertyID.Bpm, (uint)_bpm);
        } else if (context.reliableChannel) {
            LocalCacheEntry entry = _cache.localCache;
            if (entry.numberOfNodesSet || entry.bpmSet) {
                _cache.PushLocalCacheToInflight(context.updateID);
                ClearCacheOnStreamCallback(context);
            }
            if (entry.numberOfNodesSet) {
                stream.WriteVarint32((uint)PropertyID.NumberOfNodes, (uint)entry.numberOfNodes);
                didWriteProperties = true;
            }
            if (entry.bpmSet) {
                stream.WriteVarint32((uint)PropertyID.Bpm, (uint)entry.bpm);
                didWriteProperties = true;
            }
            
            if (didWriteProperties) InvalidateReliableLength();
        }
        stream.WriteCollection((uint)PropertyID.NodesArray, _nodesArray, context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            switch (propertyID) {
                case (uint)PropertyID.NodesArray: {
                    stream.ReadCollection(_nodesArray, context);
                    break;
                }
                case (uint)PropertyID.NumberOfNodes: {
                    int previousValue = _numberOfNodes;
                    _numberOfNodes = (int)stream.ReadVarint32();
                    bool numberOfNodesExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.numberOfNodesSet);
                    if (!numberOfNodesExistsInChangeCache && _numberOfNodes != previousValue) {
                        FireNumberOfNodesDidChange(_numberOfNodes);
                    }
                    break;
                }
                case (uint)PropertyID.Bpm: {
                    int previousValue = _bpm;
                    _bpm = (int)stream.ReadVarint32();
                    bool bpmExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.bpmSet);
                    if (!bpmExistsInChangeCache && _bpm != previousValue) {
                        FireBpmDidChange(_bpm);
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
        _numberOfNodes = numberOfNodes;
        _bpm = bpm;
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
