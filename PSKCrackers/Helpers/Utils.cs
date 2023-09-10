using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace PSKCrackers.Helpers
{
    public static class Utils
    {
        public static void removeVirtualProperties(object obj, ModelStateDictionary _modelState)
        {
            if(obj == null)
            {
                return;
            }
            Type type = obj.GetType();

            foreach (PropertyInfo _info in type.GetProperties())
            {
                if (_info.GetAccessors()[0].IsVirtual)
                {
                    _modelState.Remove(_info.Name);
                }
            }
        }
    }
}
