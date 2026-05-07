import { test, expect } from '@playwright/test';
import * as path from 'path';

test.describe('Calculator Application', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to the local build output
    const indexPath = path.resolve(__dirname, '../bin/Debug/netstandard2.0/h5/index.html');
    await page.goto(`file://${indexPath}`);
  });

  test('should display initial value 0', async ({ page }) => {
    await expect(page.getByText('0', { exact: true }).first()).toBeVisible();
  });

  test('should perform basic addition', async ({ page }) => {
    await page.getByRole('button', { name: '1', exact: true }).click();
    await page.getByRole('button', { name: '+', exact: true }).click();
    await page.getByRole('button', { name: '2', exact: true }).click();
    await page.getByRole('button', { name: '=', exact: true }).click();

    // Using filter and locator to reliably find the display text block
    const textBlocks = page.locator('.tss-textblock');
    await expect(textBlocks.filter({ hasText: '3' }).first()).toBeVisible();
  });

  test('should perform multiple operations', async ({ page }) => {
    // 5 * 6 = 30
    await page.getByRole('button', { name: '5', exact: true }).click();
    await page.getByRole('button', { name: 'x', exact: true }).click();
    await page.getByRole('button', { name: '6', exact: true }).click();
    await page.getByRole('button', { name: '=', exact: true }).click();

    let textBlocks = page.locator('.tss-textblock');
    await expect(textBlocks.filter({ hasText: '30' }).first()).toBeVisible();

    // 30 - 15 = 15
    await page.getByRole('button', { name: '-', exact: true }).click();
    await page.getByRole('button', { name: '1' }).first().click();
    await page.getByRole('button', { name: '5' }).first().click();
    await page.getByRole('button', { name: '=', exact: true }).click();

    textBlocks = page.locator('.tss-textblock');
    await expect(textBlocks.filter({ hasText: '15' }).first()).toBeVisible();
  });

  test('should clear correctly', async ({ page }) => {
    await page.getByRole('button', { name: '9', exact: true }).click();
    const textBlocks = page.locator('.tss-textblock');
    await expect(textBlocks.filter({ hasText: '9' }).first()).toBeVisible();

    await page.getByRole('button', { name: 'C', exact: true }).click();
    await expect(textBlocks.filter({ hasText: '0' }).first()).toBeVisible();
  });
});
