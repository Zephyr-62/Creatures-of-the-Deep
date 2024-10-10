using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    public Transform BookshelfParent;
    public Transform BookHolderParent;

    public List<Transform> Books;

    private int CurrentBookId; // == -1 when no book is held 
    private Vector3 CurrentBookShelfPos = Vector3.zero;

    public void GrabBook(int bookId)
    {
        if(bookId >= Books.Count || bookId < 0)
        {
            Debug.LogWarning("That book does not exist");
            return;
        }
        if (CurrentBookId >= 0)
            ReturnBook();


        CurrentBookId = bookId;

        var book = Books[CurrentBookId];
        CurrentBookShelfPos = book.localPosition;
        // TODO Animate claw picking up book, then:
        book.SetParent(BookHolderParent);
        book.localPosition = Vector3.zero;
    }

    public void ReturnBook()
    {
        if (CurrentBookId < 0)
        {
            Debug.LogWarning("No book being held to return");
            return;
        }

        var book = Books[CurrentBookId];
        // TODO Animate calw dropping book, then:
        book.SetParent(BookshelfParent);
        book.localPosition = CurrentBookShelfPos;
        CurrentBookId = -1;
    }

}
