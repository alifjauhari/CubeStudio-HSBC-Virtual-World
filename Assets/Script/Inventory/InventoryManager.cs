using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class Category
    {
        public string categoryName;
        public Button categoryButton;
        public Sprite selectedSprite;
        public Sprite unselectedSprite;
        public List<GameObject> itemButtons;
    }

    public Transform contentPanel;
    public Button allButton;
    public Sprite allButtonSelectedSprite;
    public Sprite allButtonUnselectedSprite;
    public List<Category> categories;

    private Button currentSelectedCategoryButton;

    private void Start()
    {
        foreach (var category in categories)
        {
            category.categoryButton.onClick.AddListener(() => SelectCategory(category));
        }

        allButton.onClick.AddListener(DisplayAllItems);

        DisplayAllItems();
    }

    public void SelectCategory(Category category)
    {
        UpdateButtonSprites(category.categoryButton, category.selectedSprite);

        FilterItemsByCategory(category.categoryName);
    }

    public void DisplayAllItems()
    {
        UpdateButtonSprites(allButton, allButtonSelectedSprite);

        ClearContentPanel();

        foreach (var category in categories)
        {
            foreach (var itemButton in category.itemButtons)
            {
                Instantiate(itemButton, contentPanel);
            }
        }
    }

    private void FilterItemsByCategory(string category)
    {
        ClearContentPanel();

        foreach (var cat in categories)
        {
            if (cat.categoryName == category)
            {
                foreach (var itemButton in cat.itemButtons)
                {
                    Instantiate(itemButton, contentPanel);
                }
                break;
            }
        }
    }

    private void ClearContentPanel()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateButtonSprites(Button selectedButton, Sprite selectedSprite)
    {
        if (currentSelectedCategoryButton != null)
        {
            Image prevButtonImage = currentSelectedCategoryButton.GetComponent<Image>();
            if (prevButtonImage != null)
            {
                prevButtonImage.sprite = currentSelectedCategoryButton == allButton ? allButtonUnselectedSprite : categories.Find(c => c.categoryButton == currentSelectedCategoryButton).unselectedSprite;
            }
        }

        Image selectedButtonImage = selectedButton.GetComponent<Image>();
        if (selectedButtonImage != null)
        {
            selectedButtonImage.sprite = selectedSprite;
        }

        currentSelectedCategoryButton = selectedButton;
    }
}
