using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Framework.ECS.Components;

namespace Framework.ECS
{
	static class Manager
	{
		private static List<string> componentNamespaces = new List<string>(){"Framework.ECS.Components"};
		private static Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>();

		private static uint entityIdCounter = 0;

		/// <summary>
		/// Manger can only work with components that he knows of. The only namespace included by default is "Framework.ECS.Components".
		/// Add you'r own namespaces with components using this method
		/// </summary>
		public static void IncludeComponentNamespace(string namespace_)
		{
			componentNamespaces.Add(namespace_);
		}
		public static List<uint> GetAllIds()
		{
			List<uint> Ids = new List<uint>();
			foreach (uint id in entities.Keys)
				Ids.Add(id);

			return Ids;
		}

		/// <summary>
		/// Creates new entity with unique ID
		/// </summary>
		/// <returns>ID of new entity</returns>
		public static uint CreateEntity()
		{
			entities.Add(entityIdCounter, new Entity());

			entityIdCounter += 1;
			return entityIdCounter - 1;
		}

		public static uint LoadEntity(string pathName)
		{
			uint entity = CreateEntity();

			StreamReader reader = new StreamReader(pathName);
            while (!reader.EndOfStream)
            {
				string line = reader.ReadLine();
				string[] parts = line.Split(" ");


				Type compType = null;
				foreach (var namespace_ in componentNamespaces)
				{
					compType = typeof(Base).Assembly.GetType($"{namespace_}.{parts[0]}");
					if (compType != null) break;
				}

				//Type compType = typeof(Base).Assembly.GetType($"{componentNamespaces[0]}.{parts[0]}");
				//if (compType == null)
				if (compType != null){
					AddComponent(entity, (Base)Activator.CreateInstance(compType));
				}
				else
				{
					throw new Exception($"Framework.ECS.Manager:LoadEntity: Unknown component - {parts[0]}. Unable to build entity from file {pathName}. ");
				}
			}
			reader.Close();

			return entity; 
		}

		/// <summary>
		/// Adds component or system to the entity, two same components or systems can not be added to the entity
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="newComponent">Component or system to add</param>
		public static void AddComponent(uint entityId, Base newComponent)
		{
			// If it is no Entity with such id - create one
			if (!entities.ContainsKey(entityId))
				CreateEntity();

			Entity targetEntity = entities[entityId];

			newComponent.Owner = targetEntity;
			targetEntity.components.Add(newComponent.Type, newComponent);
		}

		/// <summary>
		/// Returns choosed component or system of passed entity 
		/// </summary>
		/// <typeparam name="T">Must be inherited from "Base" class</typeparam>
		/// <param name="entityId"></param>
		/// <returns></returns>
		public static T GetComponent<T>(uint entityId) where T : Base
		{
			Entity e;
			if (entities.TryGetValue(entityId, out e))
			{
				T t;
				if (e.TryGetComponent<T>(out t))
					return t;
			}
			return null;
		}
	}
}
