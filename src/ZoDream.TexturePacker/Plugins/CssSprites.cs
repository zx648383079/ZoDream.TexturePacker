﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.TexturePacker.Plugins
{
    public class CssSprites(CssSpritesAlgorithm algorithm = CssSpritesAlgorithm.BinaryTree): IComparer<IImageBound>
    {
 
        /// <summary>
        /// 计算图片的位置
        /// </summary>
        /// <param name="items"></param>
        /// <returns>整个区域的宽，高</returns>
        public (float, float) Compute(IList<IImageBound> items)
        {
            return algorithm switch
            {
                CssSpritesAlgorithm.TopDown => PlaceTopDown(items),
                CssSpritesAlgorithm.LeftRight => PlaceLeftRight(items),
                CssSpritesAlgorithm.Diagonal => PlaceDiagonal(items),
                CssSpritesAlgorithm.AltDiagonal => PlaceAltDiagonal(items),
                CssSpritesAlgorithm.BinaryTree => PlaceBinaryTree(items),
                _ => (0, 0),
            };
        }

        public int Compare(IImageBound? a, IImageBound? b)
        {
            if (a == null || b == null)
            {
                return 0;
            }
            return algorithm switch
            {
                CssSpritesAlgorithm.TopDown => (int)(a.Height - b.Height),
                CssSpritesAlgorithm.LeftRight => (int)(a.Width - b.Width),
                CssSpritesAlgorithm.Diagonal or CssSpritesAlgorithm.AltDiagonal => (int)(Math.Sqrt(Math.Pow(a.Height, 2) + Math.Pow(a.Width, 2)) - Math.Sqrt(Math.Pow(b.Height, 2) + Math.Pow(b.Width, 2))),
                CssSpritesAlgorithm.BinaryTree => (int)((b.Width * b.Height) - (a.Width * a.Height)),
                _ => 0,
            };
        }

        private (float, float) PlaceTopDown(IList<IImageBound> items)
        {
            var data = items.Order(this);
            var y = 0f;
            var width = 0f;
            foreach (var item in data)
            {
                item.X = 0;
                item.Y = y;
                y += item.Height;
                if (item.Width > width)
                {
                    width = item.Width;
                }
            }
            return (width, y);
        }

        private (float, float) PlaceLeftRight(IList<IImageBound> items)
        {
            var data = items.Order(this);
            var x = 0f;
            var height = 0f;
            foreach (var item in data)
            {
                item.X = x;
                item.Y = 0;
                x += item.Width;
                if (item.Height > height)
                {
                    height = item.Height;
                }
            }
            return (x, height);
        }

        private (float, float) PlaceDiagonal(IList<IImageBound> items)
        {
            var data = items.Order(this);
            var x = 0f;
            var y = 0f;
            foreach (var item in data)
            {
                item.X = x;
                item.Y = y;
                x += item.Width;
                y += item.Height;
            }
            return (x, y);
        }

        private (float, float) PlaceAltDiagonal(IList<IImageBound> items)
        {
            var data = items.Order(this);
            var width = 0f;
            foreach (var item in data)
            {
                width += item.Width;
            }
            var x = width;
            var y = 0f;
            foreach (var item in data)
            {
                x -= item.Width;
                item.X = x;
                item.Y = y;
                y += item.Height;
            }
            return (width, y);
        }

        #region BinaryTree
        private BinaryTreeNode? _rootNode;

        private (float, float) PlaceBinaryTree(IList<IImageBound> items)
        {
            var data = items.Order(this);
            var width = items.Count > 0 ? data.First().Width : 0;
            var height = items.Count > 0 ? data.First().Height : 0;
            _rootNode = new BinaryTreeNode()
            { 
                X = 0, 
                Y = 0,
                Width = width,
                Height=  height 
            };
            var outerWidth = 0f;
            var outerHeight = 0f;
            foreach (var item in data)
            {
                var node = TreeFindNode(_rootNode, item.Width, item.Height);
                if (node is not null)
                {
                    var fit = TreeSplitNode(node, item.Width, item.Height);
                    item.X = fit.X;
                    item.Y = fit.Y;
                }
                else
                {
                    var fit = TreeGrowNode(item.Width, item.Height);
                    if (fit is not null)
                    {
                        item.X = fit.X;
                        item.Y = fit.Y;
                    }
                }
                outerWidth = Math.Max(outerWidth, item.X + item.Width);
                outerHeight = Math.Max(outerHeight, item.Y + item.Height);
            }
            _rootNode = null;
            return (outerWidth, outerHeight);
        }

        private BinaryTreeNode? TreeFindNode(
            BinaryTreeNode root, 
            float width, float height)
        {
            if (root.Used)
            {
                var node = TreeFindNode(root.Right!, width, height);
                node ??= TreeFindNode(root.Down!, width, height);
                return node;
            }
            else if ((width <= root.Width) && (height <= root.Height))
            {
                return root;
            }
            return null;
        }


        private BinaryTreeNode TreeSplitNode(
            BinaryTreeNode node, float width, float height)
        {
            node.Used = true;
            node.Down = new BinaryTreeNode() 
            { 
                X = node.X,
                Y = node.Y + height,
                Width = node.Width,
                Height = node.Height - height 
            };
            node.Right = new BinaryTreeNode() 
            { 
                X = node.X + width, 
                Y = node.Y,
                Width = node.Width - width, 
                Height = height
            };
            return node;
        }

        private BinaryTreeNode? TreeGrowNode(
            float width, float height)
        {
            var canGrowDown = (width <= _rootNode!.Width);
            var canGrowRight = (height <= _rootNode.Height);

            var shouldGrowRight = canGrowRight && (_rootNode.Height >= (_rootNode.Width + width));
            var shouldGrowDown = canGrowDown && (_rootNode.Width >= (_rootNode.Height + height));

            if (shouldGrowRight)
                return TreeGrowRight(width, height);
            else if (shouldGrowDown)
                return TreeGrowDown(width, height);
            else if (canGrowRight)
                return TreeGrowRight(width, height);
            else if (canGrowDown)
                return TreeGrowDown(width, height);
            else
                return null;
        }

        private BinaryTreeNode? TreeGrowDown(float width, float height)
        {
            _rootNode = new BinaryTreeNode()
            {
                Used = true,
                X = 0,
                Y = 0,
                Width = _rootNode!.Width,
                Height = _rootNode.Height + height,
                Right = _rootNode,
                Down = new BinaryTreeNode()
                {
                    X = 0,
                    Y = _rootNode.Height,
                    Width = _rootNode.Width,
                    Height = height
                }
            };
            var node = TreeFindNode(_rootNode, width, height);
            if (node is not null)
            {
                return TreeSplitNode(node, width, height);
            }
            return null;
        }

        private BinaryTreeNode? TreeGrowRight(float width, float height)
        {
            _rootNode = new BinaryTreeNode()
            {
                Used = true,
                X = 0,
                Y = 0,
                Width = _rootNode!.Width + width,
                Height = _rootNode.Height,
                Down = _rootNode,
                Right = new BinaryTreeNode()
                {
                    X = _rootNode.Width,
                    Y = 0,
                    Width = width,
                    Height = _rootNode.Height
                }
            };
            var node = TreeFindNode(_rootNode, width, height);
            if (node is not null)
            {
                return TreeSplitNode(node, width, height);
            }
            return null;
        }

        #endregion
    }

    public enum CssSpritesAlgorithm
    {
        TopDown,
        LeftRight,
        Diagonal,
        AltDiagonal,
        BinaryTree
    }

    internal class BinaryTreeNode
    {
        public bool Used { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public BinaryTreeNode? Down { get; set; }

        public BinaryTreeNode? Right { get; set; }
    }
}
