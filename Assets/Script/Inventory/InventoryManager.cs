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

    public Transform contentPanel; // Scroll view content panel
    public Button allButton;
    public Sprite allButtonSelectedSprite;
    public Sprite allButtonUnselectedSprite;
    public List<Category> categories;

    private Button currentSelectedCategoryButton;

    private void Start()
    {
        // Setup category buttons
        foreach (var category in categories)
        {
            category.categoryButton.onClick.AddListener(() => SelectCategory(category));
        }

        // Setup All button
        allButton.onClick.AddListener(DisplayAllItems);

        // Initially display all items and select All button
        DisplayAllItems();
    }

    public void SelectCategory(Category category)
    {
        // Update button sprites
        UpdateButtonSprites(category.categoryButton, category.selectedSprite);

        // Filter items by selected category
        FilterItemsByCategory(category.categoryName);
    }

    public void DisplayAllItems()
    {
        // Update button sprites
        UpdateButtonSprites(allButton, allButtonSelectedSprite);

        // Clear existing items
        ClearContentPanel();

        // Display all items from all categories
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
        // Clear existing items
        ClearContentPanel();

        // Find the category and display its items
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
            // Set the unselected sprite for the previously selected button
            Image prevButtonImage = currentSelectedCategoryButton.GetComponent<Image>();
            if (prevButtonImage != null)
            {
                prevButtonImage.sprite = currentSelectedCategoryButton == allButton ? allButtonUnselectedSprite : categories.Find(c => c.categoryButton == currentSelectedCategoryButton).unselectedSprite;
            }
        }

        // Set the selected sprite for the newly selected button
        Image selectedButtonImage = selectedButton.GetComponent<Image>();
        if (selectedButtonImage != null)
        {
            selectedButtonImage.sprite = selectedSprite;
        }

        currentSelectedCategoryButton = selectedButton;
    }
}
