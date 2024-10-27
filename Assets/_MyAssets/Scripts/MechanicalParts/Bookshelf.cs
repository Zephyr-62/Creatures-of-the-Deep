using AdvancedEditorTools.Attributes;
using echo17.EndlessBook;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Bookshelf;

public class Bookshelf : MonoBehaviour
{
    public Transform BookHolderParent;
    public BookholderAnimator Bookholder;

    public Material ActiveLightMat;
    public Material InactiveLightMat;

    [System.Serializable]
    public class BookSlot
    {
        [ReadOnly]
        public int id = -2; // Unassigned id
        public Button button;
        public MeshRenderer lightBulb;
        public Transform slot;
        public EndlessBook book;
        public bool active = true; // Wether the button should work or not
        public void SetLight(bool on)
        {
            lightBulb.material.SetFloat("_Intensity", on ? 1 : 0);
        }
    }
    public void SetSlotActive(BookSlot bookSlot, bool val)
    {
        if (val)
        {
            bookSlot.lightBulb.material = ActiveLightMat;
            bookSlot.SetLight(bookSlot.id == CurrentBookId);
            bookSlot.button.Unblock();
        }
        else
        {
            bookSlot.lightBulb.material = InactiveLightMat;
            bookSlot.SetLight(true);
            bookSlot.button.Block();
        }
    }

    public List<BookSlot> BookSlots;

    [SerializeField]
    [ReadOnly]
    private int CurrentBookId = -1; // == -1 when no book is held 


    private void Start()
    {
        CurrentBookId = -1;
        int id = 0;
        foreach (var bookSlot in BookSlots)
        {
            bookSlot.id = id;
            id++;
            SetSlotActive(bookSlot, bookSlot.active);
            bookSlot.book.GetComponent<BookPCS>().Block();
        }
    }

    async public void ButtonPress(int BookId)
    {
        if (BookId >= BookSlots.Count || BookId < 0)
        {
            Debug.LogWarning("That book does not exist");
            return;
        }

        if (!BookSlots[BookId].active) return;

        // Block all buttons until action is completed
        foreach (var bookSlot in BookSlots)
        {
            bookSlot.button.Block();
        }

        // Base move to position
        await Bookholder.ChangeToState(BookholderAnimator.ClawState.BookshelfInteract);

        //  Return selected book
        if (CurrentBookId == BookId)
        {
            await ReturnBook(triggerCollapse: true);
        }
        // Grab a book
        else
        {
            // Return first if already grabbing one
            if (CurrentBookId >= 0)
                await ReturnBook();
            await GrabBook(BookId);
        }

        // Unblock all buttons after action is completed
        foreach (var bookSlot in BookSlots)
        {
            if (bookSlot.active)
                bookSlot.button.Unblock();
        }
    }

    async private Task GrabBook(int bookId)
    {
        CurrentBookId = bookId;

        Bookholder.SetTargetBookSlot(BookSlots[CurrentBookId].slot);
        await Bookholder.DOTweenToBookSlot();

        var bookSlot = BookSlots[CurrentBookId];
        var book = bookSlot.book;

        // Reparent book while claw interacts
        await Bookholder.InteractWithBookshelf(() =>
        {
            book.transform.SetParent(BookHolderParent, false);
            book.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            bookSlot.SetLight(true);
        });

        await Bookholder.ChangeToState(BookholderAnimator.ClawState.BookDisplay);

        OpenBook(book);
    }

    async private Task ReturnBook(bool triggerCollapse=false)
    {
        if (CurrentBookId < 0)
        {
            Debug.LogWarning("No book being held to return");
            return;
        }

        Bookholder.SetTargetBookSlot(BookSlots[CurrentBookId].slot);

        var bookSlot = BookSlots[CurrentBookId];
        var book = bookSlot.book;
        CloseBook(book);

        await Bookholder.DOTweenToBookSlot();

        // Reparent book while claw interacts
        await Bookholder.InteractWithBookshelf(() =>
        {
            book.transform.SetParent(bookSlot.slot, false);
            book.transform.localRotation = Quaternion.identity;

            bookSlot.SetLight(false);
        });

        CurrentBookId = -1;
        if (triggerCollapse)
            await Bookholder.ChangeToState(BookholderAnimator.ClawState.Collapsed);
    }

    private void OpenBook(EndlessBook book)
    {
        book.GetComponent<BookPCS>().Unblock();
        book.SetState(EndlessBook.StateEnum.OpenFront, animationTime: 0);
        book.transform.localRotation = Quaternion.identity;
    }

    private void CloseBook(EndlessBook book)
    {
        book.GetComponent<BookPCS>().Block();
        book.SetState(EndlessBook.StateEnum.ClosedFront, animationTime: 0);
        book.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
    }

}
