﻿using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Data;

namespace NewLife.Reflection;

/// <summary>反射接口</summary>
/// <remarks>该接口仅用于扩展，不建议外部使用</remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IReflect
{
    #region 反射获取
    /// <summary>根据名称获取类型</summary>
    /// <param name="typeName">类型名</param>
    /// <param name="isLoadAssembly">是否从未加载程序集中获取类型。使用仅反射的方法检查目标类型，如果存在，则进行常规加载</param>
    /// <returns></returns>
    Type? GetType(String typeName, Boolean isLoadAssembly);

    /// <summary>获取方法</summary>
    /// <remarks>用于具有多个签名的同名方法的场合，不确定是否存在性能问题，不建议普通场合使用</remarks>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="paramTypes">参数类型数组</param>
    /// <returns></returns>
    MethodInfo? GetMethod(Type type, String name, params Type[] paramTypes);

    /// <summary>获取指定名称的方法集合，支持指定参数个数来匹配过滤</summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="paramCount">参数个数，-1表示不过滤参数个数</param>
    /// <returns></returns>
    MethodInfo[] GetMethods(Type type, String name, Int32 paramCount = -1);

    /// <summary>获取属性</summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <returns></returns>
    PropertyInfo? GetProperty(Type type, String name, Boolean ignoreCase);

    /// <summary>获取字段</summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <returns></returns>
    FieldInfo? GetField(Type type, String name, Boolean ignoreCase);

    /// <summary>获取成员</summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <returns></returns>
    MemberInfo? GetMember(Type type, String name, Boolean ignoreCase);

    /// <summary>获取字段</summary>
    /// <param name="type"></param>
    /// <param name="baseFirst"></param>
    /// <returns></returns>
    IList<FieldInfo> GetFields(Type type, Boolean baseFirst = true);

    /// <summary>获取属性</summary>
    /// <param name="type"></param>
    /// <param name="baseFirst"></param>
    /// <returns></returns>
    IList<PropertyInfo> GetProperties(Type type, Boolean baseFirst = true);
    #endregion

    #region 反射调用
    /// <summary>反射创建指定类型的实例</summary>
    /// <param name="type">类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns></returns>
    Object? CreateInstance(Type type, params Object?[] parameters);

    /// <summary>反射调用指定对象的方法</summary>
    /// <param name="target">要调用其方法的对象，如果要调用静态方法，则target是类型</param>
    /// <param name="method">方法</param>
    /// <param name="parameters">方法参数</param>
    /// <returns></returns>
    Object? Invoke(Object? target, MethodBase method, params Object?[]? parameters);

    /// <summary>反射调用指定对象的方法</summary>
    /// <param name="target">要调用其方法的对象，如果要调用静态方法，则target是类型</param>
    /// <param name="method">方法</param>
    /// <param name="parameters">方法参数字典</param>
    /// <returns></returns>
    Object? InvokeWithParams(Object? target, MethodBase method, IDictionary? parameters);

    /// <summary>获取目标对象的属性值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="property">属性</param>
    /// <returns></returns>
    Object? GetValue(Object? target, PropertyInfo property);

    /// <summary>获取目标对象的字段值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="field">字段</param>
    /// <returns></returns>
    Object? GetValue(Object? target, FieldInfo field);

    /// <summary>设置目标对象的属性值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="property">属性</param>
    /// <param name="value">数值</param>
    void SetValue(Object target, PropertyInfo property, Object? value);

    /// <summary>设置目标对象的字段值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="field">字段</param>
    /// <param name="value">数值</param>
    void SetValue(Object target, FieldInfo field, Object? value);

    /// <summary>从源对象拷贝数据到目标对象</summary>
    /// <param name="target">目标对象</param>
    /// <param name="src">源对象</param>
    /// <param name="deep">递归深度拷贝，直接拷贝成员值而不是引用</param>
    /// <param name="excludes">要忽略的成员</param>
    void Copy(Object target, Object src, Boolean deep = false, params String[] excludes);

    /// <summary>从源字典拷贝数据到目标对象</summary>
    /// <param name="target">目标对象</param>
    /// <param name="dic">源字典</param>
    /// <param name="deep">递归深度拷贝，直接拷贝成员值而不是引用</param>
    void Copy(Object target, IDictionary<String, Object?> dic, Boolean deep = false);
    #endregion

    #region 类型辅助
    /// <summary>获取一个类型的元素类型</summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    Type? GetElementType(Type type);

    /// <summary>类型转换</summary>
    /// <param name="value">数值</param>
    /// <param name="conversionType"></param>
    /// <returns></returns>
    Object? ChangeType(Object? value, Type conversionType);

    /// <summary>获取类型的友好名称</summary>
    /// <param name="type">指定类型</param>
    /// <param name="isfull">是否全名，包含命名空间</param>
    /// <returns></returns>
    String GetName(Type type, Boolean isfull);
    #endregion

    #region 插件
    /// <summary>是否能够转为指定基类</summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    Boolean As(Type type, Type baseType);

    /// <summary>在指定程序集中查找指定基类或接口的所有子类实现</summary>
    /// <param name="asm">指定程序集</param>
    /// <param name="baseType">基类或接口，为空时返回所有类型</param>
    /// <returns></returns>
    IEnumerable<Type> GetSubclasses(Assembly asm, Type baseType);

    /// <summary>在所有程序集中查找指定基类或接口的子类实现</summary>
    /// <param name="baseType">基类或接口</param>
    /// <returns></returns>
    IEnumerable<Type> GetAllSubclasses(Type baseType);

    ///// <summary>在所有程序集中查找指定基类或接口的子类实现</summary>
    ///// <param name="baseType">基类或接口</param>
    ///// <param name="isLoadAssembly">是否加载为加载程序集</param>
    ///// <returns></returns>
    //IEnumerable<Type> GetAllSubclasses(Type baseType, Boolean isLoadAssembly);
    #endregion
}

/// <summary>默认反射实现</summary>
/// <remarks>该接口仅用于扩展，不建议外部使用</remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public class DefaultReflect : IReflect
{
    #region 反射获取
    /// <summary>根据名称获取类型</summary>
    /// <param name="typeName">类型名</param>
    /// <param name="isLoadAssembly">是否从未加载程序集中获取类型。使用仅反射的方法检查目标类型，如果存在，则进行常规加载</param>
    /// <returns></returns>
    public virtual Type? GetType(String typeName, Boolean isLoadAssembly) => AssemblyX.GetType(typeName, isLoadAssembly);

    private static readonly BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    private static readonly BindingFlags bfic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

    /// <summary>获取方法</summary>
    /// <remarks>用于具有多个签名的同名方法的场合，不确定是否存在性能问题，不建议普通场合使用</remarks>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="paramTypes">参数类型数组</param>
    /// <returns></returns>
    public virtual MethodInfo? GetMethod(Type type, String name, params Type[] paramTypes)
    {
        MethodInfo? mi = null;
        while (true)
        {
            if (paramTypes == null || paramTypes.Length == 0)
                mi = type.GetMethod(name, bf);
            else
                mi = type.GetMethod(name, bf, null, paramTypes, null);
            if (mi != null) return mi;

            if (type.BaseType == null) break;
            type = type.BaseType;
            if (type == null || type == typeof(Object)) break;
        }
        return null;
    }

    /// <summary>获取指定名称的方法集合，支持指定参数个数来匹配过滤</summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="paramCount">参数个数，-1表示不过滤参数个数</param>
    /// <returns></returns>
    public virtual MethodInfo[] GetMethods(Type type, String name, Int32 paramCount = -1)
    {
        var ms = type.GetMethods(bf);
        //if (ms == null || ms.Length == 0) return ms;

        var list = new List<MethodInfo>();
        foreach (var item in ms)
        {
            if (item.Name == name)
            {
                if (paramCount >= 0 && item.GetParameters().Length == paramCount) list.Add(item);
            }
        }
        return list.ToArray();
    }

    /// <summary>获取属性</summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <returns></returns>
    public virtual PropertyInfo? GetProperty(Type type, String name, Boolean ignoreCase)
    {
        // 父类私有属性的获取需要递归，可见范围则不需要，有些类型的父类为空，比如接口
        var type2 = type;
        while (type2 != null && type2 != typeof(Object))
        {
            //var pi = type.GetProperty(name, ignoreCase ? bfic : bf);
            var pi = type2.GetProperty(name, bf);
            if (pi != null) return pi;
            if (ignoreCase)
            {
                pi = type2.GetProperty(name, bfic);
                if (pi != null) return pi;
            }

            type2 = type2.BaseType;
        }
        return null;
    }

    /// <summary>获取字段</summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <returns></returns>
    public virtual FieldInfo? GetField(Type type, String name, Boolean ignoreCase)
    {
        // 父类私有字段的获取需要递归，可见范围则不需要，有些类型的父类为空，比如接口
        var type2 = type;
        while (type2 != null && type2 != typeof(Object))
        {
            //var fi = type.GetField(name, ignoreCase ? bfic : bf);
            var fi = type2.GetField(name, bf);
            if (fi != null) return fi;
            if (ignoreCase)
            {
                fi = type2.GetField(name, bfic);
                if (fi != null) return fi;
            }

            type2 = type2.BaseType;
        }
        return null;
    }

    /// <summary>获取成员</summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="ignoreCase">忽略大小写</param>
    /// <returns></returns>
    public virtual MemberInfo? GetMember(Type type, String name, Boolean ignoreCase)
    {
        // 父类私有成员的获取需要递归，可见范围则不需要，有些类型的父类为空，比如接口
        var type2 = type;
        while (type2 != null && type2 != typeof(Object))
        {
            var fs = type2.GetMember(name, ignoreCase ? bfic : bf);
            if (fs != null && fs.Length > 0)
            {
                // 得到多个的时候，优先返回精确匹配
                if (ignoreCase && fs.Length > 1)
                {
                    foreach (var fi in fs)
                    {
                        if (fi.Name == name) return fi;
                    }
                }
                return fs[0];
            }

            type2 = type2.BaseType;
        }
        return null;
    }
    #endregion

    #region 反射获取 字段/属性
    private readonly ConcurrentDictionary<Type, IList<FieldInfo>> _cache1 = new();
    private readonly ConcurrentDictionary<Type, IList<FieldInfo>> _cache2 = new();
    /// <summary>获取字段</summary>
    /// <param name="type"></param>
    /// <param name="baseFirst"></param>
    /// <returns></returns>
    public virtual IList<FieldInfo> GetFields(Type type, Boolean baseFirst = true)
    {
        if (baseFirst)
            return _cache1.GetOrAdd(type, key => GetFields2(key, true));
        else
            return _cache2.GetOrAdd(type, key => GetFields2(key, false));
    }

    private IList<FieldInfo> GetFields2(Type type, Boolean baseFirst)
    {
        var list = new List<FieldInfo>();

        // Void*的基类就是null
        if (type == typeof(Object) || type.BaseType == null) return list;

        if (baseFirst) list.AddRange(GetFields(type.BaseType));

        var fis = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var fi in fis)
        {
            if (fi.GetCustomAttribute<NonSerializedAttribute>() != null) continue;

            list.Add(fi);
        }

        if (!baseFirst) list.AddRange(GetFields(type.BaseType));

        return list;
    }

    private readonly ConcurrentDictionary<Type, IList<PropertyInfo>> _cache3 = new();
    private readonly ConcurrentDictionary<Type, IList<PropertyInfo>> _cache4 = new();
    /// <summary>获取属性</summary>
    /// <param name="type"></param>
    /// <param name="baseFirst"></param>
    /// <returns></returns>
    public virtual IList<PropertyInfo> GetProperties(Type type, Boolean baseFirst = true)
    {
        if (baseFirst)
            return _cache3.GetOrAdd(type, key => GetProperties2(key, true));
        else
            return _cache4.GetOrAdd(type, key => GetProperties2(key, false));
    }

    private IList<PropertyInfo> GetProperties2(Type type, Boolean baseFirst)
    {
        var list = new List<PropertyInfo>();

        // Void*的基类就是null
        if (type == typeof(Object) || type.BaseType == null) return list;

        // 本身type.GetProperties就可以得到父类属性，只是不能保证父类属性在子类属性之前
        if (baseFirst) list.AddRange(GetProperties(type.BaseType));

        // 父类子类可能因为继承而有重名的属性，此时以子类优先，否则反射父类属性会出错
        var set = new HashSet<String>(list.Select(e => e.Name));

        //var pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var pi in pis)
        {
            if (pi.GetIndexParameters().Length > 0) continue;
            if (pi.GetCustomAttribute<XmlIgnoreAttribute>() != null) continue;
            if (pi.GetCustomAttribute<ScriptIgnoreAttribute>() != null) continue;
            if (pi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null) continue;

            if (!set.Contains(pi.Name))
            {
                list.Add(pi);
                set.Add(pi.Name);
            }
        }

        // 获取用于序列化的属性列表时，加上非公有的数据成员
        pis = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var pi in pis)
        {
            if (pi.GetIndexParameters().Length > 0) continue;
            if (pi.GetCustomAttribute<XmlElementAttribute>() == null && pi.GetCustomAttribute<DataMemberAttribute>() == null) continue;

            if (!set.Contains(pi.Name))
            {
                list.Add(pi);
                set.Add(pi.Name);
            }
        }

        if (!baseFirst) list.AddRange(GetProperties(type.BaseType).Where(e => !set.Contains(e.Name)));

        return list;
    }
    #endregion

    #region 反射调用
    /// <summary>反射创建指定类型的实例</summary>
    /// <param name="type">类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns></returns>
    public virtual Object? CreateInstance(Type type, params Object?[] parameters)
    {
        try
        {
            var code = type.GetTypeCode();

            // 列表
            if (code == TypeCode.Object && (type.As<IList>() || type.As(typeof(IList<>))))
            {
                var type2 = type;
                if (type2.IsInterface)
                {
                    if (type2.IsGenericType)
                        type2 = typeof(List<>).MakeGenericType(type2.GetGenericArguments());
                    else if (type2 == typeof(IList))
                        type2 = typeof(List<Object>);
                }
                return Activator.CreateInstance(type2);
            }

            // 字典
            if (code == TypeCode.Object && (type.As<IDictionary>() || type.As(typeof(IDictionary<,>))))
            {
                var type2 = type;
                if (type2.IsInterface)
                {
                    if (type2.IsGenericType)
                        type2 = typeof(Dictionary<,>).MakeGenericType(type2.GetGenericArguments());
                    else if (type2 == typeof(IDictionary))
                        type2 = typeof(Dictionary<Object, Object>);
                }
                return Activator.CreateInstance(type2);
            }

            if (parameters == null || parameters.Length == 0)
            {
                // 基元类型
                return code switch
                {
                    //TypeCode.Empty or TypeCode.DBNull => null,
                    TypeCode.Boolean => false,
                    TypeCode.Char => '\0',
                    TypeCode.SByte => (SByte)0,
                    TypeCode.Byte => (Byte)0,
                    TypeCode.Int16 => (Int16)0,
                    TypeCode.UInt16 => (UInt16)0,
                    TypeCode.Int32 => 0,
                    TypeCode.UInt32 => 0U,
                    TypeCode.Int64 => 0L,
                    TypeCode.UInt64 => 0UL,
                    TypeCode.Single => 0F,
                    TypeCode.Double => 0D,
                    TypeCode.Decimal => 0M,
                    TypeCode.DateTime => DateTime.MinValue,
                    TypeCode.String => String.Empty,
                    _ => Activator.CreateInstance(type, true),
                };
            }
            else
                return Activator.CreateInstance(type, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fail to create object type={type.FullName} parameters={parameters?.Join()} {ex.GetTrue()?.Message}", ex);
        }
    }

    /// <summary>反射调用指定对象的方法</summary>
    /// <param name="target">要调用其方法的对象，如果要调用静态方法，则target是类型</param>
    /// <param name="method">方法</param>
    /// <param name="parameters">方法参数</param>
    /// <returns></returns>
    public virtual Object? Invoke(Object? target, MethodBase method, Object?[]? parameters) => method.Invoke(target, parameters);

    /// <summary>反射调用指定对象的方法</summary>
    /// <param name="target">要调用其方法的对象，如果要调用静态方法，则target是类型</param>
    /// <param name="method">方法</param>
    /// <param name="parameters">方法参数字典</param>
    /// <returns></returns>
    public virtual Object? InvokeWithParams(Object? target, MethodBase method, IDictionary? parameters)
    {
        // 该方法没有参数，无视外部传入参数
        var pis = method.GetParameters();
        if (pis == null || pis.Length == 0) return Invoke(target, method, null);

        var ps = new Object?[pis.Length];
        for (var i = 0; i < pis.Length; i++)
        {
            Object? v = null;
            var name = pis[i].Name;
            if (parameters != null && !name.IsNullOrEmpty() && parameters.Contains(name)) v = parameters[name];
            ps[i] = v.ChangeType(pis[i].ParameterType);
        }

        return method.Invoke(target, ps);
    }

    /// <summary>获取目标对象的属性值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="property">属性</param>
    /// <returns></returns>
    public virtual Object? GetValue(Object? target, PropertyInfo property) => property.GetValue(target, null);

    /// <summary>获取目标对象的字段值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="field">字段</param>
    /// <returns></returns>
    public virtual Object? GetValue(Object? target, FieldInfo field) => field.GetValue(target);

    /// <summary>设置目标对象的属性值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="property">属性</param>
    /// <param name="value">数值</param>
    public virtual void SetValue(Object target, PropertyInfo property, Object? value) => property.SetValue(target, value.ChangeType(property.PropertyType), null);

    /// <summary>设置目标对象的字段值</summary>
    /// <param name="target">目标对象</param>
    /// <param name="field">字段</param>
    /// <param name="value">数值</param>
    public virtual void SetValue(Object target, FieldInfo field, Object? value) => field.SetValue(target, value.ChangeType(field.FieldType));
    #endregion

    #region 对象拷贝
    private static Dictionary<Type, IDictionary<String, PropertyInfo>> _properties = [];
    /// <summary>从源对象拷贝数据到目标对象</summary>
    /// <param name="target">目标对象</param>
    /// <param name="source">源对象</param>
    /// <param name="deep">递归深度拷贝，直接拷贝成员值而不是引用</param>
    /// <param name="excludes">要忽略的成员</param>
    public virtual void Copy(Object target, Object source, Boolean deep = false, params String[] excludes)
    {
        if (target == null || source == null || target == source) return;

        var targetType = target.GetType();
        // 基础类型无法拷贝
        if (targetType.IsBaseType()) throw new XException("The base type {0} cannot be copied", targetType.FullName);

        var sourceType = source.GetType();
        if (!_properties.TryGetValue(sourceType, out var sourceProperties))
            _properties[sourceType] = sourceProperties = sourceType.GetProperties(true).ToDictionary(e => e.Name, e => e);

        // 不是深度拷贝时，直接复制引用
        if (!deep)
        {
            // 借助 IModel 优化取值赋值，有 IExtend 扩展属性的实体类过于复杂而不支持，例如IEntity就有脏数据问题
            if (target is IModel dst && target is not IExtend)
            {
                foreach (var pi in targetType.GetProperties(true))
                {
                    if (!pi.CanWrite) continue;
                    if (excludes != null && excludes.Contains(pi.Name)) continue;

                    if (sourceProperties.TryGetValue(pi.Name, out var pi2) && pi2.CanRead)
                        dst[pi.Name] = source is IModel src ? src[pi2.Name] : GetValue(source, pi2);
                }
            }
            else
            {
                foreach (var pi in targetType.GetProperties(true))
                {
                    if (!pi.CanWrite) continue;
                    if (excludes != null && excludes.Contains(pi.Name)) continue;

                    if (sourceProperties.TryGetValue(pi.Name, out var pi2) && pi2.CanRead)
                        SetValue(target, pi, source is IModel src ? src[pi2.Name] : GetValue(source, pi2));
                }
            }
            return;
        }

        // 来源对象转为字典
        var dic = new Dictionary<String, Object?>();
        foreach (var pi in sourceProperties.Values)
        {
            if (!pi.CanRead) continue;
            if (excludes != null && excludes.Contains(pi.Name)) continue;

            dic[pi.Name] = GetValue(source, pi);
        }

        Copy(target, dic, deep);
    }

    /// <summary>从源字典拷贝数据到目标对象</summary>
    /// <param name="target">目标对象</param>
    /// <param name="source">源字典</param>
    /// <param name="deep">递归深度拷贝，直接拷贝成员值而不是引用</param>
    public virtual void Copy(Object target, IDictionary<String, Object?> source, Boolean deep = false)
    {
        if (target == null || source == null || source.Count == 0 || target == source) return;

        foreach (var pi in target.GetType().GetProperties(true))
        {
            if (!pi.CanWrite) continue;

            if (source.TryGetValue(pi.Name, out var obj))
            {
                // 基础类型直接拷贝，不考虑深拷贝
                if (!deep || pi.PropertyType.IsBaseType())
                    SetValue(target, pi, obj);
                else
                {
                    var v = GetValue(target, pi);

                    // 如果目标对象该成员为空，需要创建再拷贝
                    if (v == null)
                    {
                        v = pi.PropertyType.CreateInstance();
                        SetValue(target, pi, v);
                    }
                    if (v != null && obj != null) Copy(v, obj, deep);
                }
            }
        }
    }
    #endregion

    #region 类型辅助
    /// <summary>获取一个类型的元素类型</summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public virtual Type? GetElementType(Type type)
    {
        if (type.HasElementType) return type.GetElementType();

        if (type.As<IEnumerable>())
        {
            // 如果实现了IEnumerable<>接口，那么取泛型参数
            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return item.GetGenericArguments()[0];
            }
            //// 通过索引器猜测元素类型
            //var pi = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //if (pi != null) return pi.PropertyType;
        }

        return null;
    }

    /// <summary>类型转换</summary>
    /// <param name="value">数值</param>
    /// <param name="conversionType"></param>
    /// <returns></returns>
    public virtual Object? ChangeType(Object? value, Type conversionType)
    {
        // 值类型就是目标类型
        Type? vtype = null;
        if (value != null) vtype = value.GetType();
        if (vtype == conversionType) return value;

        // 可空类型
        var utype = Nullable.GetUnderlyingType(conversionType);
        if (utype != null)
        {
            if (value == null) return null;

            // 时间日期可空处理
            if (value is DateTime dt && dt == DateTime.MinValue) return null;

            conversionType = utype;
        }

        var code = Type.GetTypeCode(conversionType);
        //conversionType = Nullable.GetUnderlyingType(conversionType) ?? conversionType;
        if (conversionType.IsEnum)
        {
            if (vtype == typeof(String))
                return Enum.Parse(conversionType, (String)(value ?? String.Empty), true);
            else
                return Enum.ToObject(conversionType, value ?? 0);
        }

        // 字符串转为货币类型，处理一下
        if (vtype == typeof(String))
        {
            var str = (String)(value ?? String.Empty);
            if (code == TypeCode.Decimal)
            {
                value = str.TrimStart(['$', '￥']);
            }
            else if (conversionType.As<Type>())
            {
                return GetType(str, false);
            }

            // 字符串转为简单整型，如果长度比较小，满足32位整型要求，则先转为32位再改变类型
            if (code >= TypeCode.Int16 && code <= TypeCode.UInt64 && str.Length <= 10)
                return Convert.ChangeType(value.ToLong(), conversionType);
        }

        if (value != null)
        {
            // 尝试基础类型转换
            switch (code)
            {
                case TypeCode.Boolean:
                    return value.ToBoolean();
                case TypeCode.DateTime:
                    return value.ToDateTime();
                case TypeCode.Double:
                    return value.ToDouble();
                case TypeCode.Single:
                    return (Single)value.ToDouble();
                case TypeCode.Decimal:
                    return value.ToDecimal();
                case TypeCode.Int16:
                    return (Int16)value.ToInt();
                case TypeCode.Int32:
                    return value.ToInt();
                case TypeCode.Int64:
                    return value.ToLong();
                case TypeCode.UInt16:
                    return (UInt16)value.ToInt();
                case TypeCode.UInt32:
                    return (UInt32)value.ToInt();
                case TypeCode.UInt64:
                    return (UInt64)value.ToLong();
                default:
                    break;
            }

            // 支持DateTimeOffset转换
            if (conversionType == typeof(DateTimeOffset)) return value.ToDateTimeOffset();

            if (value is String str)
            {
                // 特殊处理几种类型，避免后续反射影响性能
                if (conversionType == typeof(Guid)) return Guid.Parse(str);
                if (conversionType == typeof(TimeSpan)) return TimeSpan.Parse(str);
#if NET5_0_OR_GREATER
                if (conversionType == typeof(IntPtr)) return IntPtr.Parse(str);
                if (conversionType == typeof(UIntPtr)) return UIntPtr.Parse(str);
                if (conversionType == typeof(Half)) return Half.Parse(str);
#endif
#if NET6_0_OR_GREATER
                if (conversionType == typeof(DateOnly)) return DateOnly.Parse(str);
                if (conversionType == typeof(TimeOnly)) return TimeOnly.Parse(str);
#endif

#if NET7_0_OR_GREATER
                // 支持IParsable<TSelf>接口
                if (conversionType.GetInterfaces().Any(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IParsable<>)))
                {
                    // 获取 TryParse 静态方法
                    var tryParse = conversionType.GetMethod("TryParse", [typeof(String), typeof(IFormatProvider), conversionType.MakeByRefType()]);
                    if (tryParse != null)
                    {
                        var parameters = new Object?[] { str, null, null };
                        var success = (Boolean)tryParse.Invoke(null, parameters)!;
                        if (success) return parameters[2];
                        //return null;
                    }
                    else
                    {
                        var mi = conversionType.GetMethod("Parse", [typeof(String), typeof(IFormatProvider)]);
                        if (mi != null) return mi.Invoke(null, [value, null]);
                    }
                }
#endif
            }

            if (value is IConvertible) value = Convert.ChangeType(value, conversionType);
        }
        else
        {
            // 如果原始值是null，要转为值类型，则new一个空白的返回
            if (conversionType.IsValueType) value = CreateInstance(conversionType);
        }

        if (conversionType.IsAssignableFrom(vtype)) return value;

        return value;
    }

    /// <summary>获取类型的友好名称</summary>
    /// <param name="type">指定类型</param>
    /// <param name="isfull">是否全名，包含命名空间</param>
    /// <returns></returns>
    public virtual String GetName(Type type, Boolean isfull) => isfull ? (type.FullName ?? type.Name) : type.Name;
    #endregion

    #region 插件
    //private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, Boolean>> _as_cache = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, Boolean>>();
    /// <summary>是否子类</summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public Boolean As(Type type, Type baseType)
    {
        if (type == null) return false;
        if (type == baseType) return true;

        // 如果基类是泛型定义，补充完整，例如IList<>
        if (baseType.IsGenericTypeDefinition
            && type.IsGenericType && !type.IsGenericTypeDefinition
            && baseType is TypeInfo inf && inf.GenericTypeParameters.Length == type.GenericTypeArguments.Length)
            baseType = baseType.MakeGenericType(type.GenericTypeArguments);

        if (type == baseType) return true;

        if (baseType.IsAssignableFrom(type)) return true;

        //// 绝大部分子类判断可通过IsAssignableFrom完成，除非其中一方ReflectionOnly
        //if (type.Assembly.ReflectionOnly == baseType.Assembly.ReflectionOnly) return false;

        // 缓存
        //var key = $"{type.FullName}_{baseType.FullName}";
        //if (!_as_cache.TryGetValue(type, out var dic))
        //{
        //    dic = new ConcurrentDictionary<Type, Boolean>();
        //    _as_cache.TryAdd(type, dic);
        //}

        //if (dic.TryGetValue(baseType, out var rs)) return rs;
        var rs = false;

        //// 接口
        //if (baseType.IsInterface)
        //{
        //    if (type.GetInterface(baseType.FullName) != null)
        //        rs = true;
        //    else if (type.GetInterfaces().Any(e => e.IsGenericType && baseType.IsGenericTypeDefinition ? e.GetGenericTypeDefinition() == baseType : e == baseType))
        //        rs = true;
        //}

        //// 判断是否子类时，支持只反射加载的程序集
        //if (!rs && type.Assembly.ReflectionOnly)
        //{
        //    // 反射加载时，需要特殊处理接口
        //    //if (baseType.IsInterface && type.GetInterface(baseType.Name) != null) return true;
        //    while (!rs && type != typeof(Object))
        //    {
        //        if (type.FullName == baseType.FullName &&
        //            type.AssemblyQualifiedName == baseType.AssemblyQualifiedName)
        //            rs = true;
        //        type = type.BaseType;
        //    }
        //}

        //dic.TryAdd(baseType, rs);

        return rs;
    }

    /// <summary>在指定程序集中查找指定基类的子类</summary>
    /// <param name="asm">指定程序集</param>
    /// <param name="baseType">基类或接口，为空时返回所有类型</param>
    /// <returns></returns>
    public virtual IEnumerable<Type> GetSubclasses(Assembly asm, Type baseType)
    {
        if (asm == null) throw new ArgumentNullException(nameof(asm));
        if (baseType == null) throw new ArgumentNullException(nameof(baseType));

        var asmx = AssemblyX.Create(asm);
        if (asmx == null) return Enumerable.Empty<Type>();

        return asmx.FindPlugins(baseType);
    }

    /// <summary>在所有程序集中查找指定基类或接口的子类实现</summary>
    /// <param name="baseType">基类或接口</param>
    /// <returns></returns>
    public virtual IEnumerable<Type> GetAllSubclasses(Type baseType)
    {
        // 不支持isLoadAssembly
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in GetSubclasses(asm, baseType))
            {
                yield return type;
            }
        }
    }

    ///// <summary>在所有程序集中查找指定基类或接口的子类实现</summary>
    ///// <param name="baseType">基类或接口</param>
    ///// <param name="isLoadAssembly">是否加载为加载程序集</param>
    ///// <returns></returns>
    //public virtual IEnumerable<Type> GetAllSubclasses(Type baseType, Boolean isLoadAssembly)
    //{
    //    //// 不支持isLoadAssembly
    //    //foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
    //    //{
    //    //    foreach (var type in GetSubclasses(asm, baseType))
    //    //    {
    //    //        yield return type;
    //    //    }
    //    //}
    //    return AssemblyX.FindAllPlugins(baseType, isLoadAssembly);
    //}
    #endregion
}