﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWebsite.Models;
using StoreWebsite.Services;

namespace StoreWebsite.Controllers
{
    public class ProductsController : Controller
    {
        readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            var model = new ProductListViewModel()
            {
                Products = products
            };

            return View(model);
        }

        [Authorize(Policy = "Management")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "Management")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (product.ImageFile == null)
                return BadRequest(new { error = "Invalid image" });

            string ext = Path.GetExtension(product.ImageFile.FileName).ToLower();

            if (ext != ".jpg" && ext != ".tiff" && ext != ".gif" && ext != ".jpeg" && ext != ".png" && ext != ".bmp")
                return BadRequest(new { error = "Invalid image format" });

            if (ModelState.IsValid)
            {
                bool result = await _productService.AddProductAsync(product);
                if (!result)
                    return BadRequest(new { error = "Could not add item" });
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        [Authorize(Policy = "Management")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var product = await _productService.GetProductAsync(Id);
            return View(product);
        }

        [Authorize(Policy = "Management")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid Id)
        {
            var product = await _productService.GetProductAsync(Id);

            if (ModelState.IsValid)
            {
                bool result = await _productService.RemoveProductAsync(product);
                if (!result)
                    return BadRequest(new { error = "Could not add item" });
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        [Authorize(Policy = "Management")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var product = await _productService.GetProductAsync(Id);
            return View(product);
        }

        [Authorize(Policy = "Management")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditConfirmed(Product product)
        {
            if (ModelState.IsValid)
            {
                bool result = await _productService.EditProductAsync(product);
                if (!result)
                    return BadRequest(new { error = "Could not add item" });
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        [Authorize(Policy = "Management")]
        public async Task<IActionResult> EditImage(Guid Id)
        {
            var product = await _productService.GetProductAsync(Id);
            return View(product);
        }

        [Authorize(Policy = "Management")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("EditImage")]
        public async Task<IActionResult> EditImageConfirmed(Product product)
        {
            if (product.ImageFile == null)
                return BadRequest(new { error = "Invalid image" });

            string ext = Path.GetExtension(product.ImageFile.FileName).ToLower();

            if (ext != ".jpg" && ext != ".tiff" && ext != ".gif" && ext != ".jpeg" && ext != ".png" && ext != ".bmp")
                return BadRequest(new { error = "Invalid image format" });

            bool result = await _productService.EditProductImageAsync(product);
            if (!result)
                return BadRequest(new { error = "Could not edit image" });
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid Id)
        {
            var product = await _productService.GetProductAsync(Id);
            return View(product);
        }
    }
}