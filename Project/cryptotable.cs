using System;
using System.Collections.Generic;
namespace CryptoTable{
    public class CryptoHashTable
    {
        private const int InitialCapacity = 16;
        private LinkedList<CryptoItem>[] buckets;
        private int _count;
        private int _capacity;

        public int Count => _count;

        public CryptoHashTable()
        {
            /*
            _________________________________________________________________
            Accepts: No parameters.

            Returns: Creates an instance of the CryptoHashTable class with an                   initial capacity of 16 and empty buckets.

            Purpose: Initializes a new hash table instance with a specified                     initial capacity and empty buckets for storing elements.
            _________________________________________________________________
            */
            _capacity = InitialCapacity;
            buckets = new LinkedList<CryptoItem>[_capacity];
            _count = 0;
        }

        private int GetBucketIndex(string key)
        {
            /*
            _________________________________________________________________
            Accepts: A string key.

            Returns: The index of the bucket where the element should be placed                 based on its key.

            Purpose: Calculates the bucket index for storing an element based on                 its key.
            _________________________________________________________________
            */
            int hashCode = GetHashCode(key);
            return hashCode % _capacity;
        }

        private int GetHashCode(string key)
        {
            /*
            _________________________________________________________________
            Accepts: A string key.

            Returns: The hash code of the string.

            Purpose: Computes the hash code of the string, which is used to                     determine the bucket index.
            _________________________________________________________________
            */
            int hash = 0;
            foreach (char c in key)
            {
                hash = (hash * 31) + c;
            }
            return hash;
        }

        public bool AddOrUpdate(string key, string value)
        {
            /*
            _________________________________________________________________
            Accepts: A key string key and a value string value.

            Returns: true if the element was added or updated; false otherwise.

            Purpose: Adds a new element or updates an existing element based on                 the given key.
            _________________________________________________________________
            */
            int bucketIndex = GetBucketIndex(key);
            if (buckets[bucketIndex] == null)
            {
                buckets[bucketIndex] = new LinkedList<CryptoItem>();
            }

            foreach (var item in buckets[bucketIndex])
            {
                if (item.Key.Equals(key))
                {
                    item.Value = value;
                    return true;
                }
            }

            if (_count >= _capacity)
            {
                IncreaseCapacity();
                bucketIndex = GetBucketIndex(key);
            }

            buckets[bucketIndex].AddLast(new CryptoItem(key, value));
            _count++;
            return true;
        }

        public bool Remove(string key)
        {
            /*
            _________________________________________________________________
            Accepts: A key string key.

            Returns: true if the element was successfully removed; false otherwise.

            Purpose: Removes an element from the hash table based on the given key.
            _________________________________________________________________
            */
            int bucketIndex = GetBucketIndex(key);
            if (buckets[bucketIndex] != null)
            {
                var bucket = buckets[bucketIndex];
                var node = bucket.First;
                while (node != null)
                {
                    if (node.Value.Key.Equals(key))
                    {
                        bucket.Remove(node);
                        _count--;
                        return true;
                    }
                    node = node.Next;
                }
            }
            return false;
        }

        public bool ContainsKey(string key)
        {
            /*
            _________________________________________________________________
            Accepts: A key string key.

            Returns: true if the key exists in the hash table; false otherwise.

            Purpose: Checks for the presence of a key in the hash table.
            _________________________________________________________________
            */
            int bucketIndex = GetBucketIndex(key);
            if (buckets[bucketIndex] != null)
            {
                foreach (var item in buckets[bucketIndex])
                {
                    if (item.Key.Equals(key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string GetValue(string key)
        {
            /*
            _________________________________________________________________
            Accepts: A key string key.

            Returns: The value associated with the given key, or throws a                         KeyNotFoundException if the key is not found.

            Purpose: Returns the value associated with the given key.
            _________________________________________________________________
            */
            int bucketIndex = GetBucketIndex(key);
            if (buckets[bucketIndex] != null)
            {
                foreach (var item in buckets[bucketIndex])
                {
                    if (item.Key.Equals(key))
                    {
                        return item.Value;
                    }
                }
            }
            throw new KeyNotFoundException($"The key '{key}' was not found in the hash table.");
        }

        public IEnumerable<string> GetKeys()
        {
            /*
            _________________________________________________________________
            Accepts: No parameters.

            Returns: An iterator over all keys in the hash table.

            Purpose: Returns all keys stored in the hash table.
            _________________________________________________________________
            */
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        yield return item.Key;
                    }
                }
            }
        }

        public IEnumerable<string> GetValues()
        {
            /*
            _________________________________________________________________
            Accepts: No parameters.

            Returns: An iterator over all values in the hash table.

            Purpose: Returns all values stored in the hash table.
            _________________________________________________________________
            */
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        yield return item.Value;
                    }
                }
            }
        }

        private void IncreaseCapacity()
        {
            /*
            _________________________________________________________________
            Accepts: No parameters.

            Returns: No return value.

            Purpose: Increases the hash table's capacity by two and redistributes elements among the new buckets.
            _________________________________________________________________
            */
            _capacity *= 2;
            var newBuckets = new LinkedList<CryptoItem>[_capacity];

            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        int newBucketIndex = GetBucketIndex(item.Key);
                        if (newBuckets[newBucketIndex] == null)
                        {
                            newBuckets[newBucketIndex] = new LinkedList<CryptoItem>();
                        }
                        newBuckets[newBucketIndex].AddLast(item);
                    }
                }
            }
            buckets = newBuckets;
        }
    }

    public class CryptoItem
    {
        public string Key { get; }
        public string Value { get; set; }

        public CryptoItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
