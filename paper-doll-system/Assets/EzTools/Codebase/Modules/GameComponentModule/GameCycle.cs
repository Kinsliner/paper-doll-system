using System.Collections.Generic;
using System.Linq;

public interface IGameCycleComponent
{
    void Init();
    void Update();
    void Uninit();
}

public class GameCycleOrderAttribute : System.Attribute
{
    public int order;
    public GameCycleOrderAttribute(int order)
    {
        this.order = order;
    }
}

public static class GameCycle
{
    public const int DefaultOrder = 1;

    public static List<IGameCycleComponent> components = new List<IGameCycleComponent>();

    public static void Init()
    {
        var types = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IGameCycleComponent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            var component = System.Activator.CreateInstance(type) as IGameCycleComponent;
            components.Add(component);
        }

        components = components.OrderBy(x =>
        {
            var attr = x.GetType().GetCustomAttributes(typeof(GameCycleOrderAttribute), false).FirstOrDefault() as GameCycleOrderAttribute;
            return attr == null ? DefaultOrder : attr.order;
        }).ToList();

        components.ForEach(x => x.Init());
    }

    public static void Update()
    {
        components.ForEach(x => x.Update());
    }

    public static void Uninit()
    {
        components.ForEach(x => x.Uninit());
    }
}