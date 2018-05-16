using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace LBE.Assets
{
    public class AssetInstantiationResult
    {
        public Object Instance = null;
        public IEnumerable<IAssetDependency> Dependencies = Enumerable.Empty<IAssetDependency>();
    }

    public class AssetInstanciator
    {
        public static AssetInstantiationResult CreateInstance(AssetDefinition assetDef, Type baseType)
        {
            var dependencies = new List<IAssetDependency>();

            Type assetType = baseType;
            if (assetDef.Type != null)
                assetType = ResolveType(assetDef.Type);

            if (assetType == null)
                return null;

            //Make sure 'T' can be assigned from the assset type                
            if (!baseType.IsAssignableFrom(assetType))
            {
                Engine.Log.Exception(String.Format("Can't assign type {0} from {1} in asset \"{2}\"", baseType.Name, assetType.Name, assetDef.Name));
                return null;
            }

            //Instantiate and initialize the new object
            if (assetType.IsArray)
            {
                Type arrayType = assetType.GetElementType();
                Array array = Array.CreateInstance(arrayType, assetDef.Fields.Values.Count);
                int idx = 0;
                foreach (Object element in assetDef.Fields.Values)
                {
                    var resolveResult = ResolveValue(element, arrayType);
                    array.SetValue(resolveResult.Instance, idx++);
                    dependencies.AddRange(resolveResult.Dependencies);
                }
                return new AssetInstantiationResult() { Instance = array, Dependencies = dependencies };
            }
            else
            {
                Object newObject = Activator.CreateInstance(assetType);

                //Set field values for the new object
                foreach (FieldInfo fieldInfo in assetType.GetFields())
                {
                    if (assetDef.Fields.Keys.Contains(fieldInfo.Name))
                    {
                        var resolveResult = ResolveValue(assetDef.Fields[fieldInfo.Name], fieldInfo.FieldType);
                        if (fieldInfo.FieldType.IsAssignableFrom(resolveResult.Instance.GetType()))
                        {
                            fieldInfo.SetValue(newObject, resolveResult.Instance);
                            dependencies.AddRange(resolveResult.Dependencies);
                        }
                        else
                        {
                            TypeConverter conv = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                            if (conv.CanConvertFrom(resolveResult.Instance.GetType()))
                            {
                                object value = conv.ConvertFrom(resolveResult.Instance);
                                fieldInfo.SetValue(newObject, value);
                                dependencies.AddRange(resolveResult.Dependencies);
                            };
                        }
                    }
                }

                //Set property values for the new object
                foreach (PropertyInfo propInfo in assetType.GetProperties())
                {
                    if (assetDef.Fields.Keys.Contains(propInfo.Name))
                    {
                        var resolveResult = ResolveValue(assetDef.Fields[propInfo.Name], propInfo.PropertyType);
                        propInfo.SetValue(newObject, resolveResult.Instance, null);
                        if (propInfo.PropertyType.IsAssignableFrom(resolveResult.Instance.GetType()))
                        {
                            propInfo.SetValue(newObject, resolveResult.Instance, null);
                            dependencies.AddRange(resolveResult.Dependencies);
                        }
                        else
                        {
                            TypeConverter conv = TypeDescriptor.GetConverter(propInfo.PropertyType);
                            if (conv.CanConvertFrom(resolveResult.Instance.GetType()))
                            {
                                object value = conv.ConvertFrom(resolveResult.Instance);
                                propInfo.SetValue(value, value, null);
                                dependencies.AddRange(resolveResult.Dependencies);
                            };
                        }
                    }
                }

                return new AssetInstantiationResult() { Instance = newObject, Dependencies = dependencies };
            }
        }

        static AssetInstantiationResult ResolveValue(Object value, Type type)
        {
            //Value is a reference to an existing asset
            if (value is AssetReference)
            {
                try
                {
                    MethodInfo resolveRefMethod = typeof(AssetInstanciator).GetMethod("ResolveReference");
                    resolveRefMethod = resolveRefMethod.MakeGenericMethod(type);
                    return resolveRefMethod.Invoke(null, new Object[] { value }) as AssetInstantiationResult;
                }
                catch (System.Exception ex)
                {
                    //If an exception occur during the Invoke() call, it will be rapper in a TargetInvocationException
                    //Just get the inner exception
                    Engine.Log.Exception(ex.InnerException);
                }
                return null;
            }
            //Value is a nested table
            else if (value is AssetDefinition)
            {
                return CreateInstance((AssetDefinition)value, type);
            }
            else
            {
                //Convert to enum type
                if (type.IsEnum)
                {
                    return new AssetInstantiationResult() { Instance = Enum.Parse(type, value as String, true) };
                }
                else if (type == typeof(int))
                {
                    return new AssetInstantiationResult() { Instance = Convert.ToInt32(value) };
                }
                else if (type == typeof(float))
                {
                    return new AssetInstantiationResult() { Instance = Convert.ToSingle(value) };
                }
                else if (type == typeof(bool))
                {
                    return new AssetInstantiationResult() { Instance = Convert.ToBoolean(value) };
                }
                else if (type == typeof(byte))
                {
                    return new AssetInstantiationResult() { Instance = Convert.ToByte(value) };
                }
                else if (type == typeof(Vector2))
                {
                    return new AssetInstantiationResult() { Instance = Vector2.Zero };
                }
                else
                {
                    return new AssetInstantiationResult() { Instance = value };
                }
            }
        }

        public static AssetInstantiationResult ResolveReference<T>(AssetReference reference)
        {
            Type refType = typeof(T);
            if (refType.IsGenericType)
            {
                var genericParams = refType.GetGenericArguments();
                var genericDef = refType.GetGenericTypeDefinition();
                if (genericDef == typeof(Asset<int>).GetGenericTypeDefinition())
                {
                    MethodInfo resolveRefMethod = typeof(AssetManager).GetMethod("GetAsset");
                    resolveRefMethod = resolveRefMethod.MakeGenericMethod(genericParams[0]);
                    var genericAsset = resolveRefMethod.Invoke(Engine.AssetManager, new Object[] { reference.Path, true });
                    return new AssetInstantiationResult() { Instance = genericAsset, Dependencies = new IAssetDependency[] { genericAsset as IAssetDependency } };
                }
            }

            Asset<T> asset = Engine.AssetManager.GetAsset<T>(reference.Path);
            return new AssetInstantiationResult() { Instance = asset.Content, Dependencies = new IAssetDependency[] { asset } };
        }

        static Type ResolveType(String typeName)
        {
            return Engine.AssetManager.TypeDatabase.GetType(typeName);
        }
    }
}
