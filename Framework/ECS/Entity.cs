using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS
{
	class Entity
	{
		public Dictionary<Type, Base> components = new Dictionary<Type, Base>();

		public bool TryGetComponent<T>(out T t) where T : Base
		{
			Base b;
			if (components.TryGetValue(typeof(T), out b))
			{
				t = (T)b;
				return true;
			}
			t = null;
			return false;
		}
	}
}
