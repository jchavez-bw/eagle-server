using EagleServer.Exceptions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace EagleServer.Helpers
{
    public static class PathTree
    {

        private static TreeNode Root { get; set; }

        public static string addPath(string path)
        {

            if (path == "/") return "/";

            //ensure uniform
            if( !path.StartsWith("/") )
            {
                path = "/" + path;
            } 

            var parts = path.Split('/');
            var pathParameters = new List<string>();

            if(Root == null )
            {
                Root = new TreeNode
                {
                    Key = ""
                };
            }

            var node = Root;

            //Start after the first '/'.  Split gives an empty "" before a '/'
            for (int i = 1; i < parts.Length; i++)
            {
                var part = parts[i];
                if (part != null && part.StartsWith("{") && part.EndsWith("}"))
                {
                    parts[i] = "*";
                    pathParameters.Add(part.Replace("{", "").Replace( "}", "" ) );
                }

                if ( !node.hasKey(parts[i]) )
                {
                    node.add(new TreeNode
                    {
                        Key = parts[i],
                    });
                }

                node = node.getNext(parts[i]);
            }

            node.IsLeaf = true;
            node.PathParameters = pathParameters;

            return string.Join("/", parts);
        }

        public static PathInfo getPath(string path)
        {
            if (path == "/") return new PathInfo { RawUrl = "/", variableUrl = "/" };

            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var parts = path.Split('/');

            var node = Root;

            var pathParameters = new List<string>();

            for (int i = 1; i < parts.Length; i++)
            {
                var part = parts[i];

                if(node.hasKey(part))
                {
                    node = node.getNext(part);
                } else if(node.hasKey("*") )
                {
                    node = node.getNext("*");
                    pathParameters.Add(part);
                    parts[i] = "*";
                } else
                {
                    //Throw 404 error
                    throw new HttpStatusAwareException(404, "Not Found");
                }
            }

            if(!node.IsLeaf) //throw 404 errr
                throw new HttpStatusAwareException(404, "Not Found");




            var pathInfo = new PathInfo(node.PathParameters, pathParameters);
            pathInfo.RawUrl = path;
            pathInfo.variableUrl = string.Join("/", parts);

            return pathInfo;
        }

        private class TreeNode
        {
            public string Key { get; set; }
            public bool IsLeaf { get; set; }

            public List<string> PathParameters { get; set; } = new List<string>();

            private Dictionary<string, TreeNode> children = new Dictionary<string, TreeNode>();

            public TreeNode() { }

            public TreeNode(string key)
            {
                this.Key = key;
            }

            public TreeNode add(TreeNode next)
            {
                children.Add(next.Key, next);
                return next;
            }

            public bool hasKey(string key )
            {
                return children.ContainsKey(key);
            }

            public TreeNode getNext(string key)
            {
                return children[key];
            }
        }
    
        public class PathInfo
        {
            public dynamic PathParameters { get; set; }

            public string RawUrl { get; set; }

            public string variableUrl { get; set; }

            public PathInfo() { }

            public PathInfo(List<string> keys, List<string> values)
            {
                if (keys.Count != values.Count)
                    throw new Exception("Path Parameters list are not the same. keys.length != value.length");

                
                var expando = new ExpandoObject();
                var dic = expando as IDictionary<string, object>;


                for (int i = 0; i < keys.Count; i++)
                {
                    dic.Add(keys[i], values[i]);                    
                }

                PathParameters = dic;
            }


        }
    }

}
