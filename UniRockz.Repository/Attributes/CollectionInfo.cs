using System;
namespace UniRockz.Repository.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionInfoAttribute : Attribute
    {
        public string CollectionName { get; }
        public string DatabaseName { get; }

        public CollectionInfoAttribute(string databaseName, string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }
    }
}
