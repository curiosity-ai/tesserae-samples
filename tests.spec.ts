import { test, expect } from '@playwright/test';
import path from 'path';

test('Image Gallery displays and opens modal', async ({ page }) => {
  const filePath = path.resolve(__dirname, 'ImageGallery/bin/Debug/netstandard2.0/h5/index.html');
  await page.goto(`file://${filePath}`);

  await expect(page.locator('text=Tesserae Image Gallery')).toBeVisible();

  const cards = page.locator('.tss-masonry-item');
  await expect(cards).toHaveCount(8);

  await cards.first().click({ force: true });
  await expect(page.locator('.tss-modal')).toBeVisible();
  await expect(page.locator('text=Image Preview')).toBeVisible();
});
