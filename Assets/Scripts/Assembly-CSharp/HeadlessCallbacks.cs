public class HeadlessCallbacks : global::System.Attribute
{
    private static global::System.Collections.IEnumerable callbackRegistry;

    public static void FindCallbacks()
    {
        if (callbackRegistry != null)
        {
            return;
        }
        try
        {
            callbackRegistry = global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Select(global::System.Reflection.Assembly.GetExecutingAssembly().GetTypes(), (global::System.Type t) => new
            {
                t,
                attributes = t.GetCustomAttributes(typeof(HeadlessCallbacks), inherit: true)
            }), _003C_003Eh__TransparentIdentifier0 => _003C_003Eh__TransparentIdentifier0.attributes != null && _003C_003Eh__TransparentIdentifier0.attributes.Length != 0), _003C_003Eh__TransparentIdentifier0 => _003C_003Eh__TransparentIdentifier0.t);
        }
        catch (global::System.Reflection.ReflectionTypeLoadException ex)
        {
            try
            {
                callbackRegistry = global::System.Linq.Enumerable.Where(ex.Types, (global::System.Type t) => t != null);
            }
            catch (global::System.Exception ex2)
            {
                global::UnityEngine.Debug.Log("Headless Builder could not find callbacks (" + ex2.GetType().Name + "), but will still continue as planned");
                callbackRegistry = global::System.Linq.Enumerable.Empty<global::System.Type>();
            }
        }
        catch (global::System.Exception ex3)
        {
            global::UnityEngine.Debug.Log("Headless Builder could not find callbacks (" + ex3.GetType().Name + "), but will still continue as planned");
            callbackRegistry = global::System.Linq.Enumerable.Empty<global::System.Type>();
        }
    }

    public static void InvokeCallbacks(string callbackName)
    {
        FindCallbacks();
        foreach (global::System.Type item in callbackRegistry)
        {
            global::System.Reflection.MethodInfo method = item.GetMethod(callbackName);
            if (method != null)
            {
                try
                {
                    method.Invoke(item, null);
                }
                catch (global::System.Exception message)
                {
                    global::UnityEngine.Debug.LogError(message);
                }
            }
        }
    }
}
