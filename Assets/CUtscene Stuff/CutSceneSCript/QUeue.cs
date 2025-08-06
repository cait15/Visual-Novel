using System;
using UnityEngine;

public class DialogueQueue<T>
{
    Node<T> rear = null;
    Node<T> front = null;

    private class Node<X>{

        public X data;
        public Node<X> next;
    }

    public bool isEmpty(){
        return front == null;
    }

    public void Enqueue(T item){
        
        Node<T> newNode = new Node<T>();

        newNode.data = item;

        if (front == null){
            front = newNode;
        }

        if (rear!= null){

            rear.next = newNode;
        }
        rear = newNode;
        Debug.Log("enqueuing data");
    }
    
    public T Dequeue(){
        if(isEmpty()){
            throw new InvalidOperationException("Can't dequeue on an empty queue.");
        }
        T Currentvalue = front.data;
        front = front.next;
        if (front == null){
            rear = null;
        }
        Debug.Log("dequeuing data");
        return Currentvalue;
    }
}