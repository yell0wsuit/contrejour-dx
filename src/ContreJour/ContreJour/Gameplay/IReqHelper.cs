using System.Collections;
using System.Collections.Generic;

namespace ContreJour.Gameplay
{
    public static class IReqHelper
    {
        public static List<object> Filter(IList objects, IReq req)
        {
            List<object> list = [];
            foreach (object @object in objects)
            {
                if (req.Meet(@object))
                {
                    list.Add(@object);
                }
            }
            return list;
        }
    }
}
