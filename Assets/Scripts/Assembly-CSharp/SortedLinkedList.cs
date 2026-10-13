using System;
using System.Collections;
using System.Collections.Generic;

public class SortedLinkedList<TKey, TValue> : IEnumerable<TValue>, IEnumerable where TValue : IComparable<TValue>
{
	private Node<TKey, TValue> head;

	private Node<TKey, TValue> tail;

	private int count;

	public int Count => count;

	public void Add(TKey key, TValue value)
	{
		Node<TKey, TValue> node = new Node<TKey, TValue>(key, value);
		if (head == null)
		{
			head = (tail = node);
		}
		else
		{
			Node<TKey, TValue> next = head;
			while (next != null && next.Value.CompareTo(value) < 0)
			{
				next = next.Next;
			}
			if (next == null)
			{
				tail.Next = node;
				node.Previous = tail;
				tail = node;
			}
			else
			{
				if (next.Previous != null)
				{
					next.Previous.Next = node;
					node.Previous = next.Previous;
				}
				else
				{
					head = node;
				}
				node.Next = next;
				next.Previous = node;
			}
		}
		count++;
	}

	public void Remove(TKey key)
	{
		for (Node<TKey, TValue> next = head; next != null; next = next.Next)
		{
			if (EqualityComparer<TKey>.Default.Equals(next.Key, key))
			{
				if (next.Previous != null)
				{
					next.Previous.Next = next.Next;
				}
				else
				{
					head = next.Next;
				}
				if (next.Next != null)
				{
					next.Next.Previous = next.Previous;
				}
				else
				{
					tail = next.Previous;
				}
				count--;
				break;
			}
		}
	}

	public void Clear()
	{
		head = null;
		tail = null;
		count = 0;
	}

	public bool TryGetByKey(TKey key, out TValue value)
	{
		for (Node<TKey, TValue> next = head; next != null; next = next.Next)
		{
			if (EqualityComparer<TKey>.Default.Equals(next.Key, key))
			{
				value = next.Value;
				return true;
			}
		}
		value = default;
		return false;
	}

	public bool TryGetFirst(out TValue value)
	{
		if (head == null)
		{
			value = default;
			return false;
		}
		value = head.Value;
		return true;
	}

	public bool TryGetLast(out TValue value)
	{
		if (tail == null)
		{
			value = default;
			return false;
		}
		value = tail.Value;
		return true;
	}

	public IEnumerator<TValue> GetEnumerator()
	{
		for (Node<TKey, TValue> current = head; current != null; current = current.Next)
		{
			yield return current.Value;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool ContainsKey(TKey key)
	{
		for (Node<TKey, TValue> next = head; next != null; next = next.Next)
		{
			if (EqualityComparer<TKey>.Default.Equals(next.Key, key))
			{
				return true;
			}
		}
		return false;
	}
}
