using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookshelf : MonoBehaviour
{
    public Transform BookshelfParent;
    public Transform BookHolderParent;


    private int _bookId;
    public int BookId
    {
        get => _bookId;
        set
        {
            if(_bookId != -1)
            {
                // Trigger return book IK
            }

            _bookId = value;
            if(_bookId != -1)
            {
                // Trigger pick book IK
            }
        }
    }




}
