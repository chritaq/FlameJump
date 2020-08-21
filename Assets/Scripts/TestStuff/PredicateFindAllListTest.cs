using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredicateFindAllListTest : MonoBehaviour
{
    private List<Book> Books = new List<Book>();
    private void Start()
    {
        Books.Add(new Book("Computer"));
        Books.Add(new Book("Romance"));
        Books.Add(new Book("Something else"));
        Books.Add(new Book("Computer"));
        List<Book> results = Books.FindAll(FindComputer);

        foreach(var books in results)
        {
            Debug.Log(books.genre);
        }
    }


    private static bool FindComputer(Book bk)
    {
        if(bk.genre == "Computer")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class Book : MonoBehaviour
{
    public Book(string newGenre)
    {
        genre = newGenre;
    }
    public string genre;
}
