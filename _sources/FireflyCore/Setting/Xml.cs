// ==========================================================================
// 
// File:        Xml.vb
// Location:    Firefly.Setting <Visual Basic .Net>
// Description: Xml读写
// Version:     2010.03.17.
// Copyright:   F.R.C.
// 
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Firefly.Texting;

namespace Firefly.Setting
{
    /// <summary>
    /// Xml
    /// 
    /// 用于将对象格式化到Xml文件及从Xml文件恢复数据
    /// 简单对象能够直接格式化
    /// 所谓的简单对象，包括String、基元类型、Enum、1维数组、有公开单参数Add方法并实现IEnumerable的简单对象元素的集合、简单对象构成的类和结构
    /// 所有的简单对象的类需要有公共的不带参数的构造函数
    /// 不能有交叉引用
    /// 允许使用继承，但所有不直接出现在根类型的类型声明的类型树中的类型必须添加到ExternalTypes中
    /// ExternalTypes中不应有命名冲突
    /// 
    /// 如果不能满足这些条件，可以使用类型替代
    /// 需要注意的是，不要在替代器中返回替代类的子类，比如，不能使用List(Of Int32)来做某个类->IEnumerable(Of Int32)的替代，这时应明确为某个类->List(Of Int32)的替代
    /// </summary>
    public sealed class Xml
    {
        private Xml()
        {
        }

        public static T ReadFile<T>(string Path)
        {
            return ReadFile<T>(Path, new Type[] { }, new IMapper[] { });
        }

        public static T ReadFile<T>(string Path, IEnumerable<Type> ExternalTypes)
        {
            return ReadFile<T>(Path, ExternalTypes, new IMapper[] { });
        }

        public static T ReadFile<T>(string Path, IEnumerable<IMapper> Mappers)
        {
            return ReadFile<T>(Path, new Type[] { }, Mappers);
        }

        public static T ReadFile<T>(string Path, IEnumerable<Type> ExternalTypes, IEnumerable<IMapper> Mappers)
        {
            using (var s = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var r = XmlReader.Create(s))
                {
                    var Root = XElement.Load(r);
                    return (T)InvMapElement("", Root, typeof(T), ExternalTypes.ToDictionary(type => GetTypeFriendlyName(type), StringComparer.OrdinalIgnoreCase), Mappers.ToDictionary(m => m.SourceType));
                }
            }
        }

        public static void WriteFile<T>(string Path, T Value)
        {
            WriteFile(Path, TextEncoding.TextEncoding.WritingDefault, Value, new Type[] { }, new IMapper[] { });
        }

        public static void WriteFile<T>(string Path, Encoding Encoding, T Value)
        {
            WriteFile(Path, Encoding, Value, new Type[] { }, new IMapper[] { });
        }

        public static void WriteFile<T>(string Path, Encoding Encoding, T Value, IEnumerable<Type> ExternalTypes)
        {
            WriteFile(Path, Encoding, Value, ExternalTypes, new IMapper[] { });
        }

        public static void WriteFile<T>(string Path, Encoding Encoding, T Value, IEnumerable<IMapper> Mappers)
        {
            WriteFile(Path, Encoding, Value, new Type[] { }, Mappers);
        }

        public static void WriteFile<T>(string Path, Encoding Encoding, T Value, IEnumerable<Type> ExternalTypes, IEnumerable<IMapper> Mappers)
        {
            var Root = MapElement("", Value, typeof(T), ExternalTypes.ToDictionary(type => GetTypeFriendlyName(type), StringComparer.OrdinalIgnoreCase), Mappers.ToDictionary(m => m.SourceType));
            var Setting = new XmlWriterSettings() { Encoding = Encoding, Indent = true, OmitXmlDeclaration = false };
            using (var tw = Txt.CreateTextWriter(Path, Encoding))
            {
                using (var w = XmlWriter.Create(tw, Setting))
                {
                    Root.Save(w);
                }
            }
        }

        private static string GetTypeFriendlyName(Type Type)
        {
            if (Type.IsArray)
            {
                int n = Type.GetArrayRank();
                string ElementTypeName = GetTypeFriendlyName(Type.GetElementType());
                if (n == 1)
                {
                    return "ArrayOf" + ElementTypeName;
                }
                return "Array" + n + "Of" + ElementTypeName;
            }
            if (Type.IsGenericType)
            {
                string Name = Regex.Match(Type.Name, "^(?<Name>.*?)`.*$", RegexOptions.ExplicitCapture).Result("${Name}");
                return Name + "Of" + string.Join("And", (from t in Type.GetGenericArguments()
                                                         select GetTypeFriendlyName(t)).ToArray());
            }
            return Type.Name;
        }

        private static XElement MapElement(string Name, object Value, Type ConstraintType, Dictionary<string, Type> ExternalTypeDict, Dictionary<Type, IMapper> MapperDict)
        {
            XElement Element;

            var PhysicalType = ConstraintType;
            if (Value is not null)
                PhysicalType = Value.GetType();

            if (string.IsNullOrEmpty(Name))
            {
                Element = new XElement(GetTypeFriendlyName(PhysicalType));
            }
            else
            {
                Element = new XElement(Name);
            }

            if (!ReferenceEquals(PhysicalType, ConstraintType))
            {
                if (!ExternalTypeDict.ContainsKey(GetTypeFriendlyName(PhysicalType)))
                    throw new InvalidOperationException("TypeNotInExternalTypes: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                Element.Attributes("Type").First().Value = GetTypeFriendlyName(PhysicalType);
            }

            // 注意：即使有转换，也保存转换之前的类型名称

            if (MapperDict.ContainsKey(PhysicalType))
            {
                var m = MapperDict[PhysicalType];
                Value = m.GetMappedObject(Value);
                PhysicalType = Value.GetType();
                ConstraintType = m.TargetType;
                if (!ReferenceEquals(PhysicalType, ConstraintType))
                    throw new NotSupportedException("MapperTypeDismatch: {0}->{1}, {2}".Formats(GetTypeFriendlyName(m.SourceType), GetTypeFriendlyName(m.TargetType), PhysicalType));
            }
            else if (MapperDict.ContainsKey(ConstraintType))
            {
                var m = MapperDict[ConstraintType];
                Value = m.GetMappedObject(Value);
                PhysicalType = Value.GetType();
                ConstraintType = m.TargetType;
                if (!ReferenceEquals(PhysicalType, ConstraintType))
                    throw new NotSupportedException("MapperTypeDismatch: {0}->{1}, {2}".Formats(GetTypeFriendlyName(m.SourceType), GetTypeFriendlyName(m.TargetType), PhysicalType));
            }

            if (Value is null)
                return Element;

            if (ReferenceEquals(PhysicalType, typeof(string)))
            {
                Element.Value = (string)Value;
                return Element;
            }

            if (ReferenceEquals(PhysicalType, typeof(decimal)))
            {
                Element.Value = ((decimal)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                return Element;
            }

            if (PhysicalType.IsPrimitive)
            {
                if (ReferenceEquals(PhysicalType, typeof(float)))
                {
                    Element.Value = ((float)Value).ToString("r", System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(double)))
                {
                    Element.Value = ((double)Value).ToString("r", System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(bool)))
                {
                    Element.Value = ((bool)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(byte)))
                {
                    Element.Value = ((byte)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(sbyte)))
                {
                    Element.Value = ((sbyte)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(short)))
                {
                    Element.Value = ((short)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(ushort)))
                {
                    Element.Value = ((ushort)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(int)))
                {
                    Element.Value = ((int)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(uint)))
                {
                    Element.Value = ((uint)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(long)))
                {
                    Element.Value = ((long)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(ulong)))
                {
                    Element.Value = ((ulong)Value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return Element;
                }

                if (ReferenceEquals(PhysicalType, typeof(IntPtr)))
                {
                    throw new NotSupportedException("IntPtrNotSerializable");
                }

                if (ReferenceEquals(PhysicalType, typeof(char)))
                {
                    throw new NotSupportedException("Char16ShallNotBeSerialized");
                }

                throw new NotSupportedException("UnexpectedPrimitive {0}".Formats(GetTypeFriendlyName(PhysicalType)));
            }

            if (PhysicalType.IsEnum)
            {
                Element.Value = Value.ToString();
                return Element;
            }

            if (PhysicalType.IsArray)
            {
                int n = PhysicalType.GetArrayRank();
                if (n != 1)
                    throw new NotSupportedException("MultidimesionArrayNotSupported: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                var ObjectType = PhysicalType.GetElementType();

                bool EmptyFlag = true;
                foreach (var o in (Array)Value)
                {
                    Element.Add(MapElement("", o, ObjectType, ExternalTypeDict, MapperDict));
                    EmptyFlag = false;
                }
                if (EmptyFlag)
                    Element.Value = "";
                return Element;
            }

            if (typeof(IEnumerable).IsAssignableFrom(PhysicalType))
            {
                IEnumerable enu = (IEnumerable)Value;
                var ObjectType = typeof(object);

                MethodInfo[] Adds = PhysicalType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(mi => mi.Name == "Add" && mi.GetParameters().Length == 1).ToArray();
                if (Adds.Length == 0)
                    throw new NotSupportedException("NoPublicOneParameterAddForIEnumerable: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                MethodInfo Add = null;
                foreach (var mi in Adds)
                {
                    var p = mi.GetParameters()[0];
                    if (ReferenceEquals(p.ParameterType, ObjectType))
                    {
                        Add = mi;
                        break;
                    }
                }
                if (Add is null)
                {
                    foreach (var mi in Adds)
                    {
                        var p = mi.GetParameters()[0];
                        if (ObjectType.IsAssignableFrom(p.ParameterType))
                        {
                            Add = mi;
                            break;
                        }
                    }
                }
                if (Add is null)
                    throw new NotSupportedException("NoCompatibleAddForIEnumerable: {0}".Formats(GetTypeFriendlyName(PhysicalType)));

                foreach (var i in PhysicalType.GetInterfaces())
                {
                    if (!i.IsGenericType)
                        continue;
                    var d = i.GetGenericTypeDefinition();
                    if (ReferenceEquals(d, typeof(IEnumerable<>)))
                    {
                        ObjectType = i.GetGenericArguments()[0];
                        break;
                    }
                }
                bool EmptyFlag = true;
                foreach (var o in enu)
                {
                    Element.Add(MapElement("", o, ObjectType, ExternalTypeDict, MapperDict));
                    EmptyFlag = false;
                }
                if (EmptyFlag)
                    Element.Value = "";
                return Element;
            }

            if (PhysicalType.IsClass)
            {
                var c = PhysicalType.GetConstructor(new Type[] { });
                if (c is null || !c.IsPublic)
                    throw new NotSupportedException("NoPublicDefaultConstructor: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                bool EmptyFlag = true;
                foreach (var f in PhysicalType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    Element.Add(MapElement(f.Name, f.GetValue(Value), f.FieldType, ExternalTypeDict, MapperDict));
                    EmptyFlag = false;
                }
                foreach (var f in PhysicalType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (f.CanRead && f.CanWrite && f.GetIndexParameters().Length == 0)
                    {
                        Element.Add(MapElement(f.Name, f.GetValue(Value, null), f.PropertyType, ExternalTypeDict, MapperDict));
                        EmptyFlag = false;
                    }
                }
                if (EmptyFlag)
                    Element.Value = "";
                return Element;
            }

            if (PhysicalType.IsValueType)
            {
                bool HasData = false;
                foreach (var f in PhysicalType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    HasData = true;
                    Element.Add(MapElement(f.Name, f.GetValue(Value), f.FieldType, ExternalTypeDict, MapperDict));
                }
                foreach (var f in PhysicalType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (f.CanRead && f.CanWrite && f.GetIndexParameters().Length == 0)
                    {
                        HasData = true;
                        Element.Add(MapElement(f.Name, f.GetValue(Value, null), f.PropertyType, ExternalTypeDict, MapperDict));
                    }
                }
                if (!HasData)
                    throw new InvalidOperationException("NonPublicValueType: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                return Element;
            }

            throw new NotSupportedException();
        }

        private static object InvMapElement(string Name, XElement Element, Type ConstraintType, Dictionary<string, Type> ExternalTypeDict, Dictionary<Type, IMapper> MapperDict)
        {
            var PhysicalType = ConstraintType;
            string value = Element.Attributes("Type").First().Value;
            if (!string.IsNullOrEmpty(value))
            {
                if (ExternalTypeDict.ContainsKey(Element.Attributes("Type").First().Value))
                {
                    PhysicalType = ExternalTypeDict[Element.Attributes("Type").First().Value];
                }
                else
                {
                    throw new InvalidOperationException("TypeNotResovled: {0}".Formats(Element.Attributes("Type")));
                }
            }

            IMapper m = null;
            if (MapperDict.ContainsKey(PhysicalType))
            {
                m = MapperDict[PhysicalType];
                PhysicalType = m.TargetType;
                ConstraintType = m.TargetType;
            }
            else if (MapperDict.ContainsKey(ConstraintType))
            {
                m = MapperDict[ConstraintType];
                PhysicalType = m.TargetType;
                ConstraintType = m.TargetType;
            }

            Func<object, object> InvM;
            if (m is null)
            {
                InvM = new Func<object, object>((o) => o);
            }
            else
            {
                InvM = new Func<object, object>((o) => m.GetInverseMappedObject(o));
            }

            if (Element.IsEmpty)
                return InvM(null);

            if (ReferenceEquals(PhysicalType, typeof(string)))
            {
                return InvM(Element.Value);
            }

            if (ReferenceEquals(PhysicalType, typeof(decimal)))
            {
                return InvM(decimal.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
            }

            if (PhysicalType.IsPrimitive)
            {
                if (ReferenceEquals(PhysicalType, typeof(float)))
                {
                    return InvM(float.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(double)))
                {
                    return InvM(double.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(bool)))
                {
                    return InvM(bool.Parse(Element.Value));
                }

                if (ReferenceEquals(PhysicalType, typeof(byte)))
                {
                    return InvM(byte.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(sbyte)))
                {
                    return InvM(sbyte.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(short)))
                {
                    return InvM(short.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(ushort)))
                {
                    return InvM(ushort.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(int)))
                {
                    return InvM(int.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(uint)))
                {
                    return InvM(uint.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(long)))
                {
                    return InvM(long.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(ulong)))
                {
                    return InvM(ulong.Parse(Element.Value, System.Globalization.CultureInfo.InvariantCulture));
                }

                if (ReferenceEquals(PhysicalType, typeof(IntPtr)))
                {
                    throw new NotSupportedException("IntPtrNotSerializable");
                }

                if (ReferenceEquals(PhysicalType, typeof(char)))
                {
                    throw new NotSupportedException("Char16ShallNotBeSerialized");
                }

                throw new NotSupportedException("UnexpectedPrimitive {0}".Formats(GetTypeFriendlyName(PhysicalType)));
            }

            if (PhysicalType.IsEnum)
            {
                return InvM(Enum.Parse(PhysicalType, Element.Value, true));
            }

            if (PhysicalType.IsArray)
            {
                int n = PhysicalType.GetArrayRank();
                if (n != 1)
                    throw new NotSupportedException("MultidimesionArrayNotSupported: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                var ObjectType = PhysicalType.GetElementType();

                XElement[] SubElements = Element.Elements().ToArray();
                var arr = Array.CreateInstance(ObjectType, SubElements.Count());
                var loopTo = arr.Length - 1;
                for (n = 0; n <= loopTo; n++)
                    arr.SetValue(InvMapElement("", SubElements[n], ObjectType, ExternalTypeDict, MapperDict), n);
                return InvM(arr);
            }

            if (typeof(IEnumerable).IsAssignableFrom(PhysicalType))
            {
                var ObjectType = typeof(object);
                foreach (var i in PhysicalType.GetInterfaces())
                {
                    if (!i.IsGenericType)
                        continue;
                    var d = i.GetGenericTypeDefinition();
                    if (ReferenceEquals(d, typeof(IEnumerable<>)))
                    {
                        ObjectType = i.GetGenericArguments()[0];
                        break;
                    }
                }

                MethodInfo[] Adds = PhysicalType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(mi => mi.Name == "Add" && mi.GetParameters().Length == 1).ToArray();
                if (Adds.Length == 0)
                    throw new NotSupportedException("NoPublicOneParameterAddForIEnumerable: {0}".Formats(GetTypeFriendlyName(PhysicalType)));
                MethodInfo Add = null;
                foreach (var mi in Adds)
                {
                    var p = mi.GetParameters()[0];
                    if (ReferenceEquals(p.ParameterType, ObjectType))
                    {
                        Add = mi;
                        break;
                    }
                }
                if (Add is null)
                {
                    foreach (var mi in Adds)
                    {
                        var p = mi.GetParameters()[0];
                        if (ObjectType.IsAssignableFrom(p.ParameterType))
                        {
                            Add = mi;
                            break;
                        }
                    }
                }
                if (Add is null)
                    throw new NotSupportedException("NoCompatibleAddForIEnumerable: {0}".Formats(GetTypeFriendlyName(PhysicalType)));

                XElement[] SubElements = Element.Elements().ToArray();
                object enu = (IEnumerable)Activator.CreateInstance(PhysicalType);
                for (int n = 0, loopTo1 = SubElements.Length - 1; n <= loopTo1; n++)
                    Add.Invoke(enu, new object[] { InvMapElement("", SubElements[n], ObjectType, ExternalTypeDict, MapperDict) });
                return InvM(enu);
            }

            if (PhysicalType.IsClass)
            {
                var c = PhysicalType.GetConstructor(new Type[] { });
                if (c is null || !c.IsPublic)
                    throw new NotSupportedException("NoPublicDefaultConstructor: {0}".Formats(GetTypeFriendlyName(PhysicalType)));

                var obj = Activator.CreateInstance(PhysicalType);

                var SubElementDict = Element.Elements().ToDictionary(e => e.Name.LocalName, StringComparer.OrdinalIgnoreCase);
                foreach (var f in PhysicalType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (SubElementDict.ContainsKey(f.Name))
                    {
                        f.SetValue(obj, InvMapElement(f.Name, SubElementDict[f.Name], f.FieldType, ExternalTypeDict, MapperDict));
                    }
                }
                foreach (var f in PhysicalType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (f.CanRead && f.CanWrite && f.GetIndexParameters().Length == 0)
                    {
                        if (SubElementDict.ContainsKey(f.Name))
                        {
                            f.SetValue(obj, InvMapElement(f.Name, SubElementDict[f.Name], f.PropertyType, ExternalTypeDict, MapperDict), null);
                        }
                    }
                }
                return InvM(obj);
            }

            if (PhysicalType.IsValueType)
            {
                // 必须定义为ValueType，否则无法正常设置值
                ValueType obj = (ValueType)Activator.CreateInstance(PhysicalType);

                var SubElementDict = Element.Elements().ToDictionary(e => e.Name.LocalName, StringComparer.OrdinalIgnoreCase);
                foreach (var f in PhysicalType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (SubElementDict.ContainsKey(f.Name))
                    {
                        f.SetValue(obj, InvMapElement(f.Name, SubElementDict[f.Name], f.FieldType, ExternalTypeDict, MapperDict));
                    }
                }
                foreach (var f in PhysicalType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (f.CanRead && f.CanWrite && f.GetIndexParameters().Length == 0)
                    {
                        if (SubElementDict.ContainsKey(f.Name))
                        {
                            f.SetValue(obj, InvMapElement(f.Name, SubElementDict[f.Name], f.PropertyType, ExternalTypeDict, MapperDict), null);
                        }
                    }
                }
                return InvM(obj);
            }

            throw new NotSupportedException();
        }

        public interface IMapper
        {
            Type SourceType { get; }
            Type TargetType { get; }
            object GetMappedObject(object o);
            object GetInverseMappedObject(object o);
        }

        public abstract class Mapper<D, R> : IMapper
        {

            public Type SourceType
            {
                get
                {
                    return typeof(D);
                }
            }

            public Type TargetType
            {
                get
                {
                    return typeof(R);
                }
            }

            public abstract R GetMappedObject(D o);

            public abstract D GetInverseMappedObject(R o);

            public object GetMappedObject(object o)
            {
                D d = (D)o;
                return GetMappedObject(d);
            }

            public object GetInverseMappedObject(object o)
            {
                R r = (R)o;
                return GetInverseMappedObject(r);
            }
        }
    }
}