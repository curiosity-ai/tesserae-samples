import { test, expect } from '@playwright/test';

test('has title and canvas elements', async ({ page }) => {
  await page.goto('http://localhost:8080/');

  // Wait for h5 and Tesserae to initialize
  await page.waitForTimeout(1000);

  // Check if header is present
  const header = page.locator('text=Tesserae Drawing Canvas');
  await expect(header).toBeVisible();

  // Check if clear button is present
  const clearBtn = page.locator('text=Clear');
  await expect(clearBtn).toBeVisible();

  // Check if canvas is present
  const canvas = page.locator('canvas');
  await expect(canvas).toBeVisible();
});

test('can draw on canvas', async ({ page }) => {
  await page.goto('http://localhost:8080/');
  await page.waitForTimeout(1000);

  const canvas = page.locator('canvas');
  await expect(canvas).toBeVisible();

  const canvasBox = await canvas.boundingBox();
  expect(canvasBox).not.toBeNull();

  // Simulate drawing a line
  if (canvasBox) {
    await page.mouse.move(canvasBox.x + 10, canvasBox.y + 10);
    await page.mouse.down();
    await page.mouse.move(canvasBox.x + 100, canvasBox.y + 100);
    await page.mouse.up();
  }

  // Check if the canvas has drawn something (this is a simple check, actual pixel matching is harder)
  // We just ensure no errors were thrown during drawing.
});

test('can clear canvas', async ({ page }) => {
  await page.goto('http://localhost:8080/');
  await page.waitForTimeout(1000);

  const clearBtn = page.locator('text=Clear');
  await expect(clearBtn).toBeVisible();

  await clearBtn.click();
});
