using Roblox;

namespace TestGame;

/// <summary>
/// Helper class for creating consistently styled UI elements.
/// </summary>
public static class UIHelper
{
    // ===========================================
    // COLOR PALETTE
    // ===========================================

    public static readonly Color3 PrimaryBg = Color3.fromRGB(30, 30, 40);
    public static readonly Color3 SecondaryBg = Color3.fromRGB(45, 45, 60);
    public static readonly Color3 AccentGreen = Color3.fromRGB(100, 200, 100);
    public static readonly Color3 AccentHover = Color3.fromRGB(120, 220, 120);
    public static readonly Color3 TextWhite = Color3.fromRGB(255, 255, 255);
    public static readonly Color3 TextGray = Color3.fromRGB(180, 180, 190);
    public static readonly Color3 GoldColor = Color3.fromRGB(255, 215, 0);
    public static readonly Color3 DisabledBg = Color3.fromRGB(80, 80, 90);
    public static readonly Color3 DisabledText = Color3.fromRGB(120, 120, 130);
    public static readonly Color3 BorderColor = Color3.fromRGB(60, 60, 80);

    // ===========================================
    // PANEL CREATION
    // ===========================================

    public static Frame CreatePanel(Instance parent, UDim2 size, UDim2 position)
    {
        var frame = InstanceFactory.Create<Frame>(parent);
        frame.Size = size;
        frame.Position = position;
        frame.BackgroundColor3 = SecondaryBg;
        frame.BorderSizePixel = 0;

        var corner = InstanceFactory.Create<UICorner>(frame);
        corner.CornerRadius = UDim.@new(0, 12);

        return frame;
    }

    public static Frame CreateMainPanel(Instance parent, UDim2 size, UDim2 position)
    {
        var frame = InstanceFactory.Create<Frame>(parent);
        frame.Name = "ShopPanel";
        frame.Size = size;
        frame.Position = position;
        frame.BackgroundColor3 = PrimaryBg;
        frame.BackgroundTransparency = 0.05f;
        frame.BorderSizePixel = 0;

        var corner = InstanceFactory.Create<UICorner>(frame);
        corner.CornerRadius = UDim.@new(0, 16);

        var stroke = InstanceFactory.Create<UIStroke>(frame);
        stroke.Color = BorderColor;
        stroke.Thickness = 2;

        return frame;
    }

    // ===========================================
    // TEXT ELEMENTS
    // ===========================================

    public static TextLabel CreateLabel(Instance parent, string text, int fontSize = 18)
    {
        var label = InstanceFactory.Create<TextLabel>(parent);
        label.Text = text;
        label.TextColor3 = TextWhite;
        label.TextSize = fontSize;
        label.BackgroundTransparency = 1;
        label.Size = UDim2.@new(1, 0, 0, fontSize + 8);

        return label;
    }

    public static TextLabel CreateHeaderLabel(Instance parent, string text)
    {
        var label = CreateLabel(parent, text, 24);
        label.Name = "Header";
        label.Size = UDim2.@new(1, 0, 0, 50);
        label.Position = UDim2.@new(0, 0, 0, 10);
        label.TextXAlignment = TextXAlignment.Center;

        return label;
    }

    // ===========================================
    // BUTTONS
    // ===========================================

    public static TextButton CreateButton(Instance parent, string text, UDim2 size)
    {
        var button = InstanceFactory.Create<TextButton>(parent);
        button.Size = size;
        button.Text = text;
        button.TextColor3 = TextWhite;
        button.BackgroundColor3 = AccentGreen;
        button.TextSize = 16;
        button.BorderSizePixel = 0;
        button.AutoButtonColor = true;

        var corner = InstanceFactory.Create<UICorner>(button);
        corner.CornerRadius = UDim.@new(0, 8);

        return button;
    }

    public static TextButton CreateToggleButton(Instance parent, UDim2 size, UDim2 position)
    {
        var button = InstanceFactory.Create<TextButton>(parent);
        button.Name = "ToggleShop";
        button.Size = size;
        button.Position = position;
        button.Text = "X";
        button.TextSize = 24;
        button.BackgroundColor3 = PrimaryBg;
        button.TextColor3 = TextWhite;
        button.BorderSizePixel = 0;
        button.AutoButtonColor = true;

        var corner = InstanceFactory.Create<UICorner>(button);
        corner.CornerRadius = UDim.@new(0, 8);

        var stroke = InstanceFactory.Create<UIStroke>(button);
        stroke.Color = BorderColor;
        stroke.Thickness = 1;

        return button;
    }

    // ===========================================
    // SCROLL FRAME
    // ===========================================

    public static ScrollingFrame CreateScrollFrame(Instance parent, UDim2 size, UDim2 position)
    {
        var scroll = InstanceFactory.Create<ScrollingFrame>(parent);
        scroll.Name = "ItemList";
        scroll.Size = size;
        scroll.Position = position;
        scroll.BackgroundTransparency = 1;
        scroll.BorderSizePixel = 0;
        scroll.ScrollBarThickness = 6;
        scroll.ScrollBarImageColor3 = TextGray;
        scroll.AutomaticCanvasSize = AutomaticSize.Y;
        scroll.CanvasSize = UDim2.@new(0, 0, 0, 0);
        scroll.ScrollingDirection = ScrollingDirection.Y;

        var layout = InstanceFactory.Create<UIListLayout>(scroll);
        layout.Padding = UDim.@new(0, 10);
        layout.SortOrder = SortOrder.LayoutOrder;
        layout.HorizontalAlignment = HorizontalAlignment.Center;

        var padding = InstanceFactory.Create<UIPadding>(scroll);
        padding.PaddingTop = UDim.@new(0, 5);
        padding.PaddingBottom = UDim.@new(0, 5);

        return scroll;
    }

    // ===========================================
    // COIN DISPLAY
    // ===========================================

    public static (Frame container, TextLabel coinLabel) CreateCoinDisplay(Instance parent, int initialCoins)
    {
        var coinFrame = InstanceFactory.Create<Frame>(parent);
        coinFrame.Name = "CoinDisplay";
        coinFrame.Size = UDim2.@new(0.9f, 0, 0, 40);
        coinFrame.Position = UDim2.@new(0.05f, 0, 0, 60);
        coinFrame.BackgroundColor3 = SecondaryBg;
        coinFrame.BorderSizePixel = 0;

        var corner = InstanceFactory.Create<UICorner>(coinFrame);
        corner.CornerRadius = UDim.@new(0, 8);

        // Coin icon (gold circle)
        var coinIcon = InstanceFactory.Create<Frame>(coinFrame);
        coinIcon.Name = "CoinIcon";
        coinIcon.Size = UDim2.@new(0, 24, 0, 24);
        coinIcon.Position = UDim2.@new(0, 10, 0.5f, -12);
        coinIcon.BackgroundColor3 = GoldColor;
        coinIcon.BorderSizePixel = 0;

        var iconCorner = InstanceFactory.Create<UICorner>(coinIcon);
        iconCorner.CornerRadius = UDim.@new(0.5f, 0);

        // Coin amount label
        var coinLabel = InstanceFactory.Create<TextLabel>(coinFrame);
        coinLabel.Name = "CoinAmount";
        coinLabel.Size = UDim2.@new(1, -50, 1, 0);
        coinLabel.Position = UDim2.@new(0, 45, 0, 0);
        coinLabel.BackgroundTransparency = 1;
        coinLabel.Text = FormatNumber(initialCoins);
        coinLabel.TextColor3 = GoldColor;
        coinLabel.TextSize = 20;
        coinLabel.TextXAlignment = TextXAlignment.Left;

        return (coinFrame, coinLabel);
    }

    // ===========================================
    // SHOP ITEM CARD
    // ===========================================

    public static (Frame card, TextButton buyButton) CreateShopItemCard(
        Instance parent,
        string name,
        string description,
        int price,
        Color3 previewColor,
        int layoutOrder)
    {
        // Main card container
        var card = InstanceFactory.Create<Frame>(parent);
        card.Name = "Item_" + name.Replace(" ", "");
        card.Size = UDim2.@new(1, -20, 0, 90);
        card.BackgroundColor3 = SecondaryBg;
        card.BorderSizePixel = 0;
        card.LayoutOrder = layoutOrder;

        var corner = InstanceFactory.Create<UICorner>(card);
        corner.CornerRadius = UDim.@new(0, 8);

        var padding = InstanceFactory.Create<UIPadding>(card);
        padding.PaddingLeft = UDim.@new(0, 10);
        padding.PaddingRight = UDim.@new(0, 10);
        padding.PaddingTop = UDim.@new(0, 10);
        padding.PaddingBottom = UDim.@new(0, 10);

        // Color preview box (left side)
        var preview = InstanceFactory.Create<Frame>(card);
        preview.Name = "Preview";
        preview.Size = UDim2.@new(0, 50, 0, 50);
        preview.Position = UDim2.@new(0, 0, 0.5f, -25);
        preview.BackgroundColor3 = previewColor;
        preview.BorderSizePixel = 0;

        var previewCorner = InstanceFactory.Create<UICorner>(preview);
        previewCorner.CornerRadius = UDim.@new(0, 6);

        // Item name
        var nameLabel = InstanceFactory.Create<TextLabel>(card);
        nameLabel.Name = "ItemName";
        nameLabel.Size = UDim2.@new(0, 120, 0, 22);
        nameLabel.Position = UDim2.@new(0, 65, 0, 5);
        nameLabel.BackgroundTransparency = 1;
        nameLabel.Text = name;
        nameLabel.TextColor3 = TextWhite;
        nameLabel.TextSize = 16;
        nameLabel.TextXAlignment = TextXAlignment.Left;
        nameLabel.TextTruncate = TextTruncate.AtEnd;

        // Price label with coin indicator
        var priceLabel = InstanceFactory.Create<TextLabel>(card);
        priceLabel.Name = "Price";
        priceLabel.Size = UDim2.@new(0, 100, 0, 18);
        priceLabel.Position = UDim2.@new(0, 65, 0, 28);
        priceLabel.BackgroundTransparency = 1;
        priceLabel.Text = FormatNumber(price) + " coins";
        priceLabel.TextColor3 = GoldColor;
        priceLabel.TextSize = 14;
        priceLabel.TextXAlignment = TextXAlignment.Left;

        // Description
        var descLabel = InstanceFactory.Create<TextLabel>(card);
        descLabel.Name = "Description";
        descLabel.Size = UDim2.@new(0, 120, 0, 16);
        descLabel.Position = UDim2.@new(0, 65, 0, 48);
        descLabel.BackgroundTransparency = 1;
        descLabel.Text = description;
        descLabel.TextColor3 = TextGray;
        descLabel.TextSize = 12;
        descLabel.TextXAlignment = TextXAlignment.Left;
        descLabel.TextTruncate = TextTruncate.AtEnd;

        // Buy button (right side)
        var buyButton = InstanceFactory.Create<TextButton>(card);
        buyButton.Name = "BuyButton";
        buyButton.Size = UDim2.@new(0, 70, 0, 35);
        buyButton.Position = UDim2.@new(1, -70, 0.5f, -17);
        buyButton.Text = "BUY";
        buyButton.TextColor3 = TextWhite;
        buyButton.BackgroundColor3 = AccentGreen;
        buyButton.TextSize = 14;
        buyButton.BorderSizePixel = 0;
        buyButton.AutoButtonColor = true;

        var buyCorner = InstanceFactory.Create<UICorner>(buyButton);
        buyCorner.CornerRadius = UDim.@new(0, 6);

        return (card, buyButton);
    }

    // ===========================================
    // UTILITIES
    // ===========================================

    public static string FormatNumber(int number)
    {
        if (number >= 1000000)
            return (number / 1000000.0).ToString("0.#") + "M";
        if (number >= 1000)
            return (number / 1000.0).ToString("0.#") + "K";
        return number.ToString();
    }

    public static void SetButtonEnabled(TextButton button, bool enabled)
    {
        button.BackgroundColor3 = enabled ? AccentGreen : DisabledBg;
        button.TextColor3 = enabled ? TextWhite : DisabledText;
        button.AutoButtonColor = enabled;
    }
}
