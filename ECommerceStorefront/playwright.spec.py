import time
from playwright.sync_api import Playwright, sync_playwright, expect
import os

def test_ecommerce_storefront(playwright: Playwright) -> None:
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()

    page.on("console", lambda msg: print("CONSOLE: ", msg.text))
    page.on("pageerror", lambda err: print("ERROR: ", err))

    page.goto("http://localhost:5000/index.html")

    page.wait_for_timeout(5000)

    # Check title
    expect(page).to_have_title("ECommerce Storefront")

    # Check header
    expect(page.locator("text=TechStore").first).to_be_visible()

    # Check product items
    products = page.locator(".tss-card")
    expect(products).to_have_count(6)

    expect(page.locator("text=Laptop Pro").first).to_be_visible()

    # Add to cart
    add_buttons = page.locator("text=Add to Cart")
    add_buttons.nth(0).click()
    page.wait_for_timeout(500)

    # Check cart badge
    badge = page.locator("text=1").first
    expect(badge).to_be_visible()

    # Add another one
    add_buttons.nth(0).click()
    page.wait_for_timeout(500)

    # Check cart badge
    badge = page.locator("text=2").first
    expect(badge).to_be_visible()

    # Open cart panel
    cart_btn = page.locator("button").first
    cart_btn.click()
    page.wait_for_timeout(1000)

    # Check cart items in panel
    cart_panel = page.locator(".tss-panel").first
    expect(cart_panel).to_be_visible()
    expect(cart_panel.locator("text=Shopping Cart").first).to_be_visible()
    expect(cart_panel.locator("text=Laptop Pro").first).to_be_visible()

    # Checkout
    checkout_btn = page.locator("text=Checkout").first
    checkout_btn.click()
    page.wait_for_timeout(1000)

    # verify cart is empty again
    expect(page.locator("text=0").first).to_be_visible()

    context.close()
    browser.close()

if __name__ == "__main__":
    with sync_playwright() as p:
        test_ecommerce_storefront(p)
