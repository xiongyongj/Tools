using System.Collections.Generic;
using UnityEngine;
namespace BookCurlPro.Examples
{
    /// <summary>
    /// How to use it:
    /// 1) Create a new gameobject in the scene and attach this script on it
    /// 2) Create a prefab for the front and back pages(should be similar to the the page object that is created by the editor and has the same components)
    /// 3) Assign the front and back prefabs to this component
    /// 4) Call the function AddPaper when you want to add a new paper in the book
    /// 5) you may need to customize this function to add a paper in specific position in the book or to change some parts of the front and back prefabs dynamically
    /// </summary>
    public class AddPagesDynamically : MonoBehaviour
    {
        public GameObject FrontPagePrefab;
        public GameObject BackPagePrefab;
        public void AddPaper(BookPro book)
        {
            GameObject frontPage = Instantiate(FrontPagePrefab);
            GameObject backPage = Instantiate(BackPagePrefab);
            frontPage.transform.SetParent(book.transform, false);
            backPage.transform.SetParent(book.transform, false);
            Paper newPaper = new Paper();
            newPaper.Front = frontPage;
            newPaper.Back = backPage;
            List<Paper> papers = new(book.papers.Count + 1);
            for (int i = 0; i < book.papers.Count; i++)
            {
                papers[i] = book.papers[i];
            }
            papers[papers.Count - 1] = newPaper;
            book.papers = papers;
            //update the flipping range to contain the new added paper
            book.EndFlippingPaper = book.papers.Count - 1;
            book.UpdatePages();
        }

    }
}
