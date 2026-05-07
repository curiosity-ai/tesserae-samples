const { test, expect } = require('@playwright/test');

test('AddressBook loads and functions properly', async ({ page }) => {
  await page.goto('http://localhost:8080');

  // Wait for the search box
  await page.waitForSelector('input[placeholder="Search contacts..."]');

  // Wait for at least one contact card to load
  await page.waitForSelector('text=Clementina DuBuque');

  // The user should exist in the default load
  await expect(page.locator('text=Clementina DuBuque').first()).toBeVisible();

  // Type into the search box
  const searchBox = page.locator('input[placeholder="Search contacts..."]');
  await searchBox.fill('Clementina');

  // Wait for filter, there should be one user
  await expect(page.locator('text=Clementina DuBuque')).toBeVisible();

  // Click on Clementina
  await page.locator('text=Clementina DuBuque').click();

  // Check details
  await expect(page.locator('text=@Moriah.Stanton')).toBeVisible(); // Clementina's username
});
