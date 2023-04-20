using System.Collections.Generic;

public static class ObjectReusePool<T> where T : new()
{
	private static Queue<T> _pool;

	private static Queue<T> Pool
	{
		get
		{
			if (_pool == null)
			{
				_pool = new Queue<T>();
			}
			return _pool;
		}
	}

	public static T Spawn()
	{
		T newObject;
		if (Pool.Count > 0)
		{
			newObject = Pool.Dequeue();
		}
		else
		{
			newObject = new T();
		}
		return newObject;
	}

	public static void Despawn(T target)
	{
		Pool.Enqueue(target);
	}
}
