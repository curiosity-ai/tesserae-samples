using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;

namespace ECommerceStorefront
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    class Program
    {
        static List<Product> _products = new List<Product>();
        static List<CartItem> _cart = new List<CartItem>();
        static Stack _cartPanelContent = VStack();
        static Tesserae.Panel _cartPanel;
        static TextBlock _cartCountBadge;
        static Stack _productGrid;

        static void Main(string[] args)
        {
            document.body.style.overflow = "hidden";
            document.body.style.margin = "0";

            Theme.Dark();

            _cartCountBadge = TextBlock("0").Primary().SemiBold();

            var header = HStack().JustifyContent(ItemJustify.Between).AlignItems(ItemAlign.Center).Padding(16.px()).Background("var(--tss-default-bg-color)")
                .Children(
                    TextBlock("TechStore").XLarge().Bold().Primary(),
                    HStack().AlignItems(ItemAlign.Center).Children(
                        Button().SetIcon(UIcons.ShoppingCart).OnClick((s, e) => _cartPanel.Show()),
                        _cartCountBadge
                    )
                );

            _productGrid = HStack().Wrap().JustifyContent(ItemJustify.Center).Padding(16.px());

            _cartPanel = Tesserae.UI.Panel().LightDismiss();
            _cartPanel.Side = Tesserae.Panel.PanelSide.Far;
            _cartPanel.Width(400.px()).Content(_cartPanelContent);

            var page = VStack().Children(
                header,
                VStack().ScrollY().Children(_productGrid)
            ).Height(100.vh()).Width(100.vw());

            document.body.appendChild(page.Render());
            UpdateCartUI();

            Task.Run(LoadProductsAsync);
        }

        static void LoadProductsAsync()
        {
            try
            {
                var xhr = new H5.Core.dom.XMLHttpRequest();
                xhr.open("GET", "products.json");
                xhr.onload = (e) =>
                {
                    if (xhr.status == 200)
                    {
                        var json = xhr.responseText;
                        var parsed = JSON.parse(json);
                        var arr = parsed.As<Product[]>();

                        _products = new List<Product>(arr);
                        RenderProducts();
                    }
                    else
                    {
                        console.log("Error loading products: ", xhr.status);
                    }
                };
                xhr.send();
            }
            catch (Exception ex)
            {
                console.log("Error loading products:");
                console.log(ex);
                Toast().Error($"Error loading products: {ex.Message}");
            }
        }

        static void RenderProducts()
        {
            _productGrid.Clear();
            foreach (var product in _products)
            {
                var card = Card(
                    VStack().Children(
                        Image(product.ImageUrl).Width(200.px()).Height(200.px()).PT(16.px()).PB(16.px()),
                        TextBlock(product.Name).Large().Bold(),
                        TextBlock(product.Description).Small().PT(8.px()).PB(8.px()).Style(s => s.color = "var(--tss-text-color-secondary)"),
                        HStack().JustifyContent(ItemJustify.Between).AlignItems(ItemAlign.Center).PT(16.px()).Children(
                            TextBlock($"${product.Price}").Medium().Bold().Primary(),
                            Button("Add to Cart").Primary().OnClick((s, e) => AddToCart(product))
                        )
                    )
                ).Width(250.px()).Margin(16.px());

                _productGrid.Add(card);
            }
        }

        static void AddToCart(Product product)
        {
            var existingItem = _cart.FirstOrDefault(item => item.Product.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _cart.Add(new CartItem { Product = product, Quantity = 1 });
            }

            UpdateCartUI();
            Toast().Success($"Added {product.Name} to cart");
        }

        static void UpdateCartUI()
        {
            _cartPanelContent.Clear();

            _cartPanelContent.Add(TextBlock("Shopping Cart").XLarge().Bold().PB(16.px()));

            if (_cart.Count == 0)
            {
                _cartPanelContent.Add(TextBlock("Your cart is empty.").Medium());
                _cartCountBadge.Text = "0";
                return;
            }

            int totalCount = 0;
            double totalPrice = 0;

            foreach (var item in _cart.ToList())
            {
                totalCount += item.Quantity;

                // Hack: JSON parsing may have created Price as primitive double instead of System.Decimal type wrapper.
                // We use double explicitly here to avoid runtime JS TypeError (mul is not a function).
                double p = item.Product.Price.As<double>();
                totalPrice += p * item.Quantity;

                var itemRow = HStack().JustifyContent(ItemJustify.Between).AlignItems(ItemAlign.Center).PB(8.px()).MB(8.px()).Children(
                    HStack().AlignItems(ItemAlign.Center).Children(
                        Image(item.Product.ImageUrl).Width(50.px()).Height(50.px()).MR(8.px()),
                        VStack().Children(
                            TextBlock(item.Product.Name).Medium().Bold(),
                            TextBlock($"${p} x {item.Quantity}").Small()
                        )
                    ),
                    HStack().AlignItems(ItemAlign.Center).Children(
                        Button().SetIcon(UIcons.Minus).OnClick((s, e) => DecreaseQuantity(item)),
                        TextBlock(item.Quantity.ToString()).Medium().Padding("0 8px"),
                        Button().SetIcon(UIcons.Plus).OnClick((s, e) => IncreaseQuantity(item)),
                        Button().SetIcon(UIcons.Trash).Danger().OnClick((s, e) => RemoveFromCart(item)).ML(8.px())
                    )
                );

                _cartPanelContent.Add(itemRow);
            }

            _cartCountBadge.Text = totalCount.ToString();

            _cartPanelContent.Add(
                HStack().JustifyContent(ItemJustify.Between).PT(16.px()).PB(16.px()).Children(
                    TextBlock("Total:").Large().Bold(),
                    TextBlock($"${totalPrice:F2}").Large().Bold().Primary()
                )
            );

            _cartPanelContent.Add(
                Button("Checkout").Primary().WS().OnClick((s, e) => {
                    Toast().Success("Checkout simulated!");
                    _cart.Clear();
                    UpdateCartUI();
                    _cartPanel.Hide();
                })
            );
        }

        static void IncreaseQuantity(CartItem item)
        {
            item.Quantity++;
            UpdateCartUI();
        }

        static void DecreaseQuantity(CartItem item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                _cart.Remove(item);
            }
            UpdateCartUI();
        }

        static void RemoveFromCart(CartItem item)
        {
            _cart.Remove(item);
            UpdateCartUI();
        }
    }
}
