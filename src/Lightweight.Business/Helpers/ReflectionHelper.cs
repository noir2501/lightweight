using System;
using Lightweight.Business.Exceptions;

namespace Lightweight.Business.Helpers
{
    public class ReflectionHelper
    {
        public static object GetPropertyValue(object obj, string property, bool allownull = false)
        {
            if (string.IsNullOrEmpty(property))
                throw new BusinessException("Failed to retrieve property value. Property name not specified.");

            if (obj == null)
                if (!allownull)
                    throw new BusinessException(string.Format("Trying to get property named {0} from object failed. Object was null.", property));
                else
                    return null;

            var propList = property.Split('.');
            Type rootType = obj.GetType();

            Type type = rootType;
            object current = obj;

            foreach (var propName in propList)
            {
                var prop = type.GetProperty(propName);

                if (prop == null)
                    throw new BusinessException(string.Format("No property named {0} was found on object of type {1} while retrieving {2} property from object of type {3}.", propName, type.Name, property, rootType.Name));

                type = prop.PropertyType;
                current = prop.GetValue(current, null);

                if (current == null)
                    if (!allownull)
                        throw new BusinessException(string.Format("Failed to retrieve property value for {0}. {1} is null and property is not optional.", property, propName));
                    else
                        return null;
            }

            return current;
        }

        public static void SetPropertyValue(object obj, string property, object value)
        {
            if (string.IsNullOrEmpty(property))
                throw new BusinessException("Failed to set property value. Property name not specified.");

            var propList = property.Split('.');
            Type rootType = obj.GetType();

            Type type = rootType;
            object current = obj;

            for (int i = 0; i < propList.Length; i++)
            {
                var propName = propList[i];
                var prop = type.GetProperty(propName);

                if (prop == null)
                    throw new BusinessException(string.Format("No property named {0} was found on object of type {1} while setting {2} property's valuue on object of type {3}.", propName, type.Name, property, rootType.Name));

                if (i < propList.Length - 1)
                {
                    type = prop.PropertyType;
                    current = prop.GetValue(current, null);

                    if (current == null)
                        throw new BusinessException(string.Format("Failed to set property value. Property {0} of type {1} is null on object of type {2}", propName, type.Name, rootType.Name));
                }
                else // this is the last property in chain so we should set it
                    if (!prop.PropertyType.IsInstanceOfType(value)) // if properties are not of same type....
                        try // try to convert from source type to destination type
                        {
                            object newvalue = Convert.ChangeType(value, prop.PropertyType);
                            prop.SetValue(current, newvalue, null);
                        }
                        catch (Exception e)
                        {
                            throw new BusinessException(string.Format("Failed to set property value. Property and value types do not match. {0}", e.Message));
                        }
                    else
                        prop.SetValue(current, value, null);
            }
        }
    }
}
