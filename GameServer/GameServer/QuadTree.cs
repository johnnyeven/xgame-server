using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace com.xgame.GameServer.data
{
    public class QuadTree<T> where T : class
    {
        private readonly int _MaxDepth;
        internal readonly Dictionary<T, QuadTreeNode> _Table;
        private QuadTreeNode _RootNode;

        public Rectangle Bounds { get; private set; }

        public QuadTree(Rectangle bounds, byte maxDepth)
        {
            if (maxDepth <= 0)
            {
                throw new ArgumentException("maxDepth", "maxDepth cannot be less than 1.");
            }

            Bounds = bounds;
            _MaxDepth = maxDepth;
            _Table = new Dictionary<T, QuadTreeNode>();
            _RootNode = new QuadTreeNode(bounds);
        }

        public QuadTree(Rectangle bounds)
            : this(bounds, 255)
        { }

        public void Insert(T item, Rectangle bounds)
        {
            Insert(new QuadNodeItem(item, bounds));
        }

        public void Remove(T item)
        {
            QuadTreeNode node;
            if (_Table.TryGetValue(item, out node))
            {
                node.RemoveItem(item);
                _Table.Remove(item);
            }
        }

        public IEnumerable<T> GetInsideItems(Rectangle bounds)
        {
            return _RootNode.GetInsideItems(ref bounds);
        }

        public IEnumerable<T> GetIntersectedItems(Rectangle bounds)
        {
            return _RootNode.GetIntersectedItems(ref bounds);
        }

        public void Restructure(Rectangle bounds)
        {
            Bounds = bounds;
            _RootNode = new QuadTreeNode(bounds);
            QuadTreeNode[] nodes = _Table.Values.ToArray();

            foreach (QuadTreeNode node in nodes)
            {
                foreach (var nodeItem in node.Items)
                {
                    Insert(nodeItem);
                }
            }
        }

        public bool PredicateItemsCount(Rectangle bounds, int thresholdCount)
        {
            if (thresholdCount <= 0)
            {
                throw new ArgumentException("thresholdCount", "thresholdCount cannot be less or equal than 0.");
            }

            int count = 0;
            bool ret = _RootNode.PredicateItemsCount(ref bounds, thresholdCount, ref count);
            return ret;
        }

        private void Insert(QuadNodeItem item)
        {
            if (!IsvalidBounds(item.Bounds))
            {
                throw new ArgumentException("bounds");
            }

            Rectangle bounds = item.Bounds;
            QuadTreeNode node = _RootNode.Insert(item, ref bounds, 1, _MaxDepth);
            _Table[item.Datum] = node;
        }

        private bool IsvalidBounds(Rectangle bounds)
        {
            return IsValidBound(bounds.X) && IsValidBound(bounds.Y)
              && IsValidBound(bounds.Width) && IsValidBound(bounds.Height);
        }

        private bool IsValidBound(double boundValue)
        {
            return !double.IsNaN(boundValue) && !double.IsInfinity(boundValue);
        }

        internal class QuadTreeNode
        {
            private List<QuadNodeItem> _Items = new List<QuadNodeItem>();
            private Rectangle _Bounds;
            private QuadTreeNode _TopLeftNode;
            private QuadTreeNode _TopRightNode;
            private QuadTreeNode _BottomLeftNode;
            private QuadTreeNode _BottomRightNode;

            public List<QuadNodeItem> Items
            {
                get { return _Items; }
            }

            public Rectangle Bounds
            {
                get { return _Bounds; }
            }

            public QuadTreeNode(Rectangle bounds)
            {
                _Bounds = bounds;
            }

            #region Insert

            public QuadTreeNode Insert(QuadNodeItem item, ref Rectangle bounds, int depth, int maxDepth)
            {
                if (depth < maxDepth)
                {
                    QuadTreeNode child = GetItemContainerNode(ref bounds);

                    if (child != null)
                    {
                        return child.Insert(item, ref bounds, depth + 1, maxDepth);
                    }
                }

                _Items.Add(item);
                return this;
            }

            private QuadTreeNode GetItemContainerNode(ref Rectangle bounds)
            {
                int halfWidth = _Bounds.Width / 2;
                int halfHeight = _Bounds.Height / 2;

                QuadTreeNode child = null;

                if (_TopLeftNode == null)
                {
                    Rectangle topLeftRectangle = new Rectangle(_Bounds.X, _Bounds.Y, halfWidth, halfHeight);

                    if (topLeftRectangle.Contains(bounds))
                    {
                        _TopLeftNode = new QuadTreeNode(topLeftRectangle);
                        child = _TopLeftNode;
                    }
                }
                else if (_TopLeftNode._Bounds.Contains(bounds))
                {
                    child = _TopLeftNode;
                }

                if (child == null)
                {
                    if (_TopRightNode == null)
                    {
                        Rectangle topRightRectangle = new Rectangle(_Bounds.X + halfWidth, _Bounds.Y, halfWidth, halfHeight);

                        if (topRightRectangle.Contains(bounds))
                        {
                            _TopRightNode = new QuadTreeNode(topRightRectangle);
                            child = _TopRightNode;
                        }
                    }
                    else if (_TopRightNode._Bounds.Contains(bounds))
                    {
                        child = _TopRightNode;
                    }
                }

                if (child == null)
                {
                    if (_BottomRightNode == null)
                    {
                        Rectangle bottomRightRectangle = new Rectangle(_Bounds.X + halfWidth, _Bounds.Y + halfHeight, halfWidth, halfHeight);

                        if (bottomRightRectangle.Contains(bounds))
                        {
                            _BottomRightNode = new QuadTreeNode(bottomRightRectangle);
                            child = _BottomRightNode;
                        }
                    }
                    else if (_BottomRightNode._Bounds.Contains(bounds))
                    {
                        child = _BottomRightNode;
                    }
                }

                if (child == null)
                {
                    if (_BottomLeftNode == null)
                    {
                        Rectangle bottomLeftRectangle = new Rectangle(_Bounds.X, _Bounds.Y + halfHeight, halfWidth, halfHeight);

                        if (bottomLeftRectangle.Contains(bounds))
                        {
                            _BottomLeftNode = new QuadTreeNode(bottomLeftRectangle);
                            child = _BottomLeftNode;
                        }
                    }
                    else if (_BottomLeftNode._Bounds.Contains(bounds))
                    {
                        child = _BottomLeftNode;
                    }
                }
                return child;
            }

            #endregion

            #region Get Inside Items

            public IEnumerable<T> GetInsideItems(ref Rectangle bounds)
            {
                if (!bounds.IntersectsWith(_Bounds))
                {
                    return Enumerable.Empty<T>();
                }

                List<T> containedNodes = new List<T>();

                if (bounds.Contains(_Bounds))
                {
                    GetItemWithoutCheck(containedNodes);
                    return containedNodes;
                }

                if (_TopLeftNode != null && _TopLeftNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _TopLeftNode.GetInsideItems(ref bounds);
                    containedNodes.AddRange(items);
                }

                if (_TopRightNode != null && _TopRightNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _TopRightNode.GetInsideItems(ref bounds);
                    containedNodes.AddRange(items);
                }

                if (_BottomRightNode != null && _BottomRightNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _BottomRightNode.GetInsideItems(ref bounds);
                    containedNodes.AddRange(items);
                }

                if (_BottomLeftNode != null && _BottomLeftNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _BottomLeftNode.GetInsideItems(ref bounds);
                    containedNodes.AddRange(items);
                }

                GetContainedItems(ref bounds, containedNodes);

                return containedNodes;
            }

            private void GetContainedItems(ref Rectangle bounds, List<T> nodes)
            {
                foreach (QuadNodeItem item in _Items)
                {
                    if (bounds.Contains(item.Bounds))
                    {
                        nodes.Add(item.Datum);
                    }
                }
            }

            #endregion

            #region Get Intersected Items

            public IEnumerable<T> GetIntersectedItems(ref Rectangle bounds)
            {
                if (!bounds.IntersectsWith(_Bounds))
                {
                    return Enumerable.Empty<T>();
                }

                List<T> intersectedNodes = new List<T>();

                if (bounds.Contains(_Bounds))
                {
                    GetItemWithoutCheck(intersectedNodes);
                    return intersectedNodes;
                }

                if (_TopLeftNode != null && _TopLeftNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _TopLeftNode.GetIntersectedItems(ref bounds);
                    intersectedNodes.AddRange(items);
                }

                if (_TopRightNode != null && _TopRightNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _TopRightNode.GetIntersectedItems(ref bounds);
                    intersectedNodes.AddRange(items);
                }

                if (_BottomRightNode != null && _BottomRightNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _BottomRightNode.GetIntersectedItems(ref bounds);
                    intersectedNodes.AddRange(items);
                }

                if (_BottomLeftNode != null && _BottomLeftNode._Bounds.IntersectsWith(bounds))
                {
                    var items = _BottomLeftNode.GetIntersectedItems(ref bounds);
                    intersectedNodes.AddRange(items);
                }

                GetIntersectedItems(ref bounds, intersectedNodes);

                return intersectedNodes;
            }

            private void GetIntersectedItems(ref Rectangle bounds, List<T> nodes)
            {
                foreach (QuadNodeItem item in _Items)
                {
                    if (bounds.IntersectsWith(item.Bounds))
                    {
                        nodes.Add(item.Datum);
                    }
                }
            }

            #endregion

            public void RemoveItem(T item)
            {
                int itemIndex = _Items.FindIndex(nodeItem => nodeItem.Datum == item);
                _Items.RemoveAt(itemIndex);
            }

            #region Predicate Item Count

            public bool PredicateItemsCount(ref Rectangle bounds, int thresholdCount, ref int count)
            {
                if (!bounds.IntersectsWith(_Bounds))
                {
                    return true;
                }

                if (bounds.Contains(_Bounds))
                {
                    return PredicateItemsCountWithoutCheck(thresholdCount, ref count);
                }

                if (!PredicateIntersectedItemsCount(ref bounds, thresholdCount, ref count))
                {
                    return false;
                }

                if (_TopLeftNode != null && _TopLeftNode._Bounds.IntersectsWith(bounds))
                {
                    if (!_TopLeftNode.PredicateItemsCount(ref bounds, thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                if (_TopRightNode != null && _TopRightNode._Bounds.IntersectsWith(bounds))
                {
                    if (!_TopRightNode.PredicateItemsCount(ref bounds, thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                if (_BottomRightNode != null && _BottomRightNode._Bounds.IntersectsWith(bounds))
                {
                    if (!_BottomRightNode.PredicateItemsCount(ref bounds, thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                if (_BottomLeftNode != null && _BottomLeftNode._Bounds.IntersectsWith(bounds))
                {
                    if (!_BottomLeftNode.PredicateItemsCount(ref bounds, thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool PredicateItemsCountWithoutCheck(int thresholdCount, ref int count)
            {
                count += _Items.Count;

                if (count > thresholdCount)
                {
                    return false;
                }

                if (_TopLeftNode != null)
                {
                    if (!_TopLeftNode.PredicateItemsCountWithoutCheck(thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                if (_TopRightNode != null)
                {
                    if (!_TopRightNode.PredicateItemsCountWithoutCheck(thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                if (_BottomLeftNode != null)
                {
                    if (!_BottomLeftNode.PredicateItemsCountWithoutCheck(thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                if (_BottomRightNode != null)
                {
                    if (!_BottomRightNode.PredicateItemsCountWithoutCheck(thresholdCount, ref count))
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool PredicateIntersectedItemsCount(ref Rectangle bounds, int thresholdCount, ref int count)
            {
                foreach (QuadNodeItem item in _Items)
                {
                    if (bounds.IntersectsWith(item.Bounds))
                    {
                        if (++count > thresholdCount)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            #endregion

            private void GetItemWithoutCheck(List<T> nodes)
            {
                nodes.AddRange(_Items.Select(item => item.Datum));

                if (_TopLeftNode != null)
                {
                    _TopLeftNode.GetItemWithoutCheck(nodes);
                }

                if (_TopRightNode != null)
                {
                    _TopRightNode.GetItemWithoutCheck(nodes);
                }

                if (_BottomLeftNode != null)
                {
                    _BottomLeftNode.GetItemWithoutCheck(nodes);
                }

                if (_BottomRightNode != null)
                {
                    _BottomRightNode.GetItemWithoutCheck(nodes);
                }
            }
        }

        internal class QuadNodeItem
        {
            public QuadNodeItem(T item, Rectangle bounds)
            {
                Datum = item;
                Bounds = bounds;
            }

            public T Datum { get; private set; }
            public Rectangle Bounds { get; private set; }
        }
    }
}
