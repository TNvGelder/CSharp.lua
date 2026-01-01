using Roblox;
using System;
using System.Collections.Generic;

namespace TestGame;

/// <summary>
/// Represents a purchasable item in the shop.
/// </summary>
public class ShopItem
{
    public string Id { get; }
    public string DisplayName { get; }
    public int Price { get; }
    public string Description { get; }
    public Color3 PreviewColor { get; }
    public Action<Vector3> SpawnAction { get; }

    public ShopItem(string id, string displayName, int price, string description,
                    Color3 previewColor, Action<Vector3> spawnAction)
    {
        Id = id;
        DisplayName = displayName;
        Price = price;
        Description = description;
        PreviewColor = previewColor;
        SpawnAction = spawnAction;
    }
}

/// <summary>
/// Main shop UI controller. Creates and manages the building shop interface.
/// </summary>
public class ShopUI
{
    private readonly Player _player;
    private readonly CurrencyManager _currencyManager;
    private readonly ScreenGui _screenGui;
    private readonly Frame _shopPanel;
    private readonly TextButton _toggleButton;
    private readonly TextLabel _coinLabel;
    private readonly ScrollingFrame _itemList;
    private bool _isShopOpen = true;

    private readonly List<ShopItem> _shopItems;
    private readonly Dictionary<string, TextButton> _buyButtons = new();

    /// <summary>
    /// Creates the shop UI for a specific player.
    /// </summary>
    public ShopUI(Player player)
    {
        _player = player;
        _currencyManager = new CurrencyManager(10000);
        _shopItems = CreateShopItems();

        // Create main ScreenGui
        _screenGui = InstanceFactory.Create<ScreenGui>(player.PlayerGui);
        _screenGui.Name = "BuildingShop";
        _screenGui.IgnoreGuiInset = true;
        _screenGui.DisplayOrder = 10;
        _screenGui.ResetOnSpawn = false;

        // Create main shop panel (right side of screen)
        _shopPanel = CreateShopPanel();

        // Create toggle button
        _toggleButton = CreateToggleButton();

        // Create currency display
        _coinLabel = CreateCurrencyDisplay();

        // Create scrollable item list
        _itemList = CreateItemList();

        // Populate items
        PopulateShopItems();

        // Subscribe to currency updates
        _currencyManager.OnCoinsChanged += UpdateCurrencyDisplay;
        _currencyManager.OnCoinsChanged += UpdateAllButtonStates;

        // Start passive income
        _currencyManager.StartPassiveIncome();

        System.Console.WriteLine("Shop UI initialized");
    }

    // ===========================================
    // SHOP ITEMS DEFINITION
    // ===========================================

    private List<ShopItem> CreateShopItems()
    {
        return new List<ShopItem>
        {
            new ShopItem(
                "tree",
                "Tree",
                100,
                "A simple tree",
                Color3.fromRGB(45, 95, 35),
                BuildingSpawner.SpawnTree
            ),
            new ShopItem(
                "well",
                "Well",
                300,
                "Stone well with water",
                Color3.fromRGB(140, 135, 125),
                BuildingSpawner.SpawnWell
            ),
            new ShopItem(
                "small_house",
                "House (Small)",
                500,
                "Cozy timber house",
                Color3.fromRGB(245, 235, 210),
                BuildingSpawner.SpawnSmallHouse
            ),
            new ShopItem(
                "market_stall",
                "Market Stall",
                800,
                "Colorful vendor stall",
                Color3.fromRGB(180, 40, 45),
                BuildingSpawner.SpawnMarketStall
            ),
            new ShopItem(
                "large_house",
                "House (Large)",
                1000,
                "Spacious family home",
                Color3.fromRGB(250, 245, 235),
                BuildingSpawner.SpawnLargeHouse
            ),
            new ShopItem(
                "tower",
                "Tower",
                1500,
                "Tall stone watchtower",
                Color3.fromRGB(95, 90, 85),
                BuildingSpawner.SpawnTower
            ),
            new ShopItem(
                "windmill",
                "Windmill",
                2500,
                "Working grain mill",
                Color3.fromRGB(180, 175, 165),
                BuildingSpawner.SpawnWindmill
            ),
            new ShopItem(
                "church",
                "Church",
                3000,
                "Grand village church",
                Color3.fromRGB(180, 175, 165),
                BuildingSpawner.SpawnChurch
            ),
        };
    }

    // ===========================================
    // UI CREATION
    // ===========================================

    private Frame CreateShopPanel()
    {
        var panel = UIHelper.CreateMainPanel(
            _screenGui,
            UDim2.@new(0.25, 0, 0.8, 0),
            UDim2.@new(0.74, 0, 0.1, 0)
        );

        // Header
        UIHelper.CreateHeaderLabel(panel, "Building Shop");

        return panel;
    }

    private TextButton CreateToggleButton()
    {
        var button = UIHelper.CreateToggleButton(
            _screenGui,
            UDim2.@new(0, 50, 0, 50),
            UDim2.@new(0.72, -10, 0.1, 0)
        );

        button.MouseButton1Click.Connect(ToggleShop);

        return button;
    }

    private TextLabel CreateCurrencyDisplay()
    {
        var (container, label) = UIHelper.CreateCoinDisplay(_shopPanel, _currencyManager.Coins);
        return label;
    }

    private ScrollingFrame CreateItemList()
    {
        return UIHelper.CreateScrollFrame(
            _shopPanel,
            UDim2.@new(0.9, 0, 0.72, 0),
            UDim2.@new(0.05, 0, 0, 115)
        );
    }

    private void PopulateShopItems()
    {
        int order = 0;
        foreach (var item in _shopItems)
        {
            var (card, buyButton) = UIHelper.CreateShopItemCard(
                _itemList,
                item.DisplayName,
                item.Description,
                item.Price,
                item.PreviewColor,
                order
            );

            // Store button reference for state updates
            _buyButtons[item.Id] = buyButton;

            // Set initial button state
            UIHelper.SetButtonEnabled(buyButton, _currencyManager.CanAfford(item.Price));

            // Connect buy action
            var capturedItem = item;  // Capture for closure
            buyButton.MouseButton1Click.Connect(() => OnBuyClicked(capturedItem));

            order++;
        }
    }

    // ===========================================
    // EVENT HANDLERS
    // ===========================================

    private void ToggleShop()
    {
        _isShopOpen = !_isShopOpen;
        _shopPanel.Visible = _isShopOpen;

        if (_isShopOpen)
        {
            _toggleButton.Text = "X";
            _toggleButton.Size = UDim2.@new(0, 50, 0, 50);
        }
        else
        {
            _toggleButton.Text = "Shop";
            _toggleButton.Size = UDim2.@new(0, 70, 0, 40);
        }
    }

    private void OnBuyClicked(ShopItem item)
    {
        if (!_currencyManager.CanAfford(item.Price))
        {
            System.Console.WriteLine($"Cannot afford {item.DisplayName}!");
            return;
        }

        if (_currencyManager.TrySpend(item.Price))
        {
            // Get spawn position in front of player
            var spawnPos = BuildingSpawner.GetSpawnPosition(_player);

            // Spawn the building
            item.SpawnAction(spawnPos);

            System.Console.WriteLine($"Purchased {item.DisplayName} for {item.Price} coins!");
        }
    }

    private void UpdateCurrencyDisplay(int newAmount)
    {
        _coinLabel.Text = UIHelper.FormatNumber(newAmount);
    }

    private void UpdateAllButtonStates(int currentCoins)
    {
        foreach (var item in _shopItems)
        {
            if (_buyButtons.TryGetValue(item.Id, out var button))
            {
                UIHelper.SetButtonEnabled(button, currentCoins >= item.Price);
            }
        }
    }

    // ===========================================
    // STATIC INITIALIZATION
    // ===========================================

    /// <summary>
    /// Initializes the shop UI for a player.
    /// Call this when a player joins the game.
    /// </summary>
    public static void Initialize(Player player)
    {
        new ShopUI(player);
    }
}
