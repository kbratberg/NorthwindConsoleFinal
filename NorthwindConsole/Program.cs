﻿using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Edit a Category");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories");
                    Console.WriteLine("5) Display All Categories and their Active Products");
                    Console.WriteLine("6) Display Specific Product details");
                    Console.WriteLine("7) Add Product");
                    Console.WriteLine("8) Edit a Product");
                    Console.WriteLine("9) Display all Products");
                    Console.WriteLine("10) Delete a Category");
                    Console.WriteLine("11) Delete a Product");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();


                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        //edit Category
                        var db = new NorthwindConsole_32_KMBContext();
                        Category category = GetCategory(db);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Details:\nName: {category.CategoryName}\nDescription: {category.Description}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Category UpdatedCategory = InputCategory(db);
                        
                        if(category != null){
                            category.CategoryName = UpdatedCategory.CategoryName;
                            if(UpdatedCategory.Description != null){
                            category.Description = UpdatedCategory.Description;
                            }
                            db.SaveChanges();
                        }
                        logger.Info($"Updated Category: {category.CategoryId} Name: {category.CategoryName}Description: {category.Description}");
                    }
                    else if (choice == "2")
                    {   //add Category
                        var db = new NorthwindConsole_32_KMBContext();
                        Category category = InputCategory(db);

                        if (category != null)
                        {
                            db.Categories.Add(category);
                            db.SaveChanges();
                        }
                        logger.Info($"Added Category: {category.CategoryId} {category.CategoryName} {category.Description}");
                    }
                    else if (choice == "3")
                    {   //display category and related products
                        var db = new NorthwindConsole_32_KMBContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        var count = 0;
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                            count++;
                        }
                        logger.Info($"{count} product items in {category.CategoryName}");
                    }
                    if (choice == "4")
                    {
                        // display all categories
                        var db = new NorthwindConsole_32_KMBContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "5")
                    {
                        //Display all categories and their related active products
                        var db = new NorthwindConsole_32_KMBContext();
                        var categoryQuery = db.Categories.Include("Products").OrderBy(p => p.CategoryName);
                        
                        foreach (var item in categoryQuery)
                        {   
                            var count = 0;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{item.CategoryName}");
                            Console.ForegroundColor = ConsoleColor.White;
                            foreach (Product p in item.Products)
                            {
                                if (p.Discontinued == false)
                                {
                                    Console.WriteLine($"\t{p.ProductName}");
                                    count++;
                                }
                            }
                            logger.Info($"{count} active product/s items in {item.CategoryName}");
                        }

                        Console.WriteLine();
                    }
                    else if (choice == "6")
                    {
                        //  display products
                        var db = new NorthwindConsole_32_KMBContext();
                        var query = db.Products.OrderBy(p => p.ProductName);
                        var product = GetProduct(db);
                        logger.Info($"{query.Count()} Products Listed");
                        if (product != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"ProductId: {product.ProductId}\nProduct Name: {product.ProductName}\nSupplierId: {product.SupplierId}\nCategoryId: {product.CategoryId}\nQuantity Per Unit: {product.QuantityPerUnit}");
                            Console.WriteLine($"Unit Price: {product.UnitPrice}\nUnits in Stock: {product.UnitsInStock}\nUnits On Order: {product.UnitsOnOrder}\nReorder Level: {product.ReorderLevel}");
                            if (product.Discontinued == true)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Discontinued");
                            }
                            else if (product.Discontinued == false)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Active");
                            }
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else if (choice == "7")
                    {
                        // add a product
                        var db = new NorthwindConsole_32_KMBContext();
                        Product product = InputProduct(db);

                        if (product != null)
                        {
                            db.Products.Add(product);
                            db.SaveChanges();
                        }
                        logger.Info($"Product Added");
                        logger.Info($"Product Name: {product.ProductName}\nSupplierId: {product.SupplierId}\nCategoryId: {product.CategoryId}\nQuantity Per Unit: {product.QuantityPerUnit}");
                        logger.Info($"Unit Price: {product.UnitPrice}\nUnits in Stock: {product.UnitsInStock}\nUnits On Order: {product.UnitsOnOrder}\nReorder Level: {product.ReorderLevel}");
                    }
                    else if (choice == "8")
                    {
                        //edit a product
                        var db = new NorthwindConsole_32_KMBContext();
                        Product product= GetProduct(db);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Product Details");
                        Console.WriteLine($"ProductId: {product.ProductId}\nProduct Name: {product.ProductName}\nSupplierId: {product.SupplierId}\nCategoryId: {product.CategoryId}\nQuantity Per Unit: {product.QuantityPerUnit}");
                        Console.WriteLine($"Unit Price: {product.UnitPrice}\nUnits in Stock: {product.UnitsInStock}\nUnits On Order: {product.UnitsOnOrder}\nReorder Level: {product.ReorderLevel}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Product UpdatedProduct = InputProduct(db);
                        
                        if(product != null){
                            if(UpdatedProduct.ProductName != null){
                            product.ProductName = UpdatedProduct.ProductName;
                            }
                            if(UpdatedProduct.SupplierId != null){
                            product.SupplierId = UpdatedProduct.SupplierId;
                            }
                            if(UpdatedProduct.CategoryId != null){
                            product.CategoryId = UpdatedProduct.CategoryId;
                            }
                            if(UpdatedProduct.UnitPrice != null){
                            product.UnitPrice = UpdatedProduct.UnitPrice;
                            }
                            if(UpdatedProduct.QuantityPerUnit != null){
                            product.QuantityPerUnit = UpdatedProduct.QuantityPerUnit;
                            }
                            if(UpdatedProduct.UnitsInStock != null){
                            product.UnitsInStock = UpdatedProduct.UnitsInStock;
                            }
                            if(UpdatedProduct.UnitsOnOrder != null){
                            product.UnitsOnOrder = UpdatedProduct.UnitsOnOrder;
                            }
                            if(UpdatedProduct.ReorderLevel != null){
                            product.ReorderLevel = UpdatedProduct.ReorderLevel;
                            }
                            
                            product.Discontinued = UpdatedProduct.Discontinued;
                            
                            db.SaveChanges();
                        }
                        logger.Info($"Updated product: {product.ProductId}");
                        logger.Info($"Product Name: {product.ProductName}\nSupplierId: {product.SupplierId}\nCategoryId: {product.CategoryId}\nQuantity Per Unit: {product.QuantityPerUnit}");
                        logger.Info($"Unit Price: {product.UnitPrice}\nUnits in Stock: {product.UnitsInStock}\nUnits On Order: {product.UnitsOnOrder}\nReorder Level: {product.ReorderLevel}");

                    }
                    else if (choice == "9")
                    {
                        // display products
                        //pick an option to display
                        Console.WriteLine("Select Which Products to Display");
                        Console.WriteLine("1)Display all Products");
                        Console.WriteLine("2)Display Active Products");
                        Console.WriteLine("3)Display Discontinued Products");

                        var displayChoice = Console.ReadLine();
                        logger.Info($"Your choice: {displayChoice}");

                        // log error if invalid choice

                        if (displayChoice != "1" || displayChoice != "2" || displayChoice != "3")
                        {
                            logger.Info("Invalid Choice");
                        }
                        var db = new NorthwindConsole_32_KMBContext();

                        if (displayChoice == "1")
                        {
                            // display all Products Name only
                            var query = db.Products.OrderBy(p => p.ProductName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"{query.Count()} Products");
                            foreach (var item in query)
                            {
                                if (item.Discontinued == true)
                                {
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        if (displayChoice == "2")
                        {
                            var query = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"{query.Count()} Products");
                            foreach (var item in query)
                            {

                                Console.WriteLine($"{item.ProductName}");

                            }

                        }
                        if (displayChoice == "3")
                        {
                            var query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.White;
                            logger.Info($"{query.Count()} Products");
                            foreach (var item in query)
                            {

                                Console.WriteLine($"{item.ProductName}");

                            }

                        }
                    }
                    else if (choice == "10")
                    {
                        //delete category
                        Console.WriteLine("Choose the product to delete:");
                         var db = new NorthwindConsole_32_KMBContext();
                            Category category = GetCategory(db);
                        if (category != null)
                        {
                            var productQuery = db.Categories.Include("Products").FirstOrDefault(p => p.CategoryId == category.CategoryId);
                             
                             foreach(Product product in category.Products){
                                 if(product.CategoryId == category.CategoryId)
                                    product.CategoryId = null;
                             }
                            db.Categories.Remove(category);
                            db.SaveChanges();
                            logger.Info($"Category (id: {category.CategoryId}) deleted");
                        }
                    }else if (choice == "11")
                    {
                        //delete product
                        Console.WriteLine("Choose the product to delete:");
                         var db = new NorthwindConsole_32_KMBContext();
                        
                        var product = GetProduct(db);
                        if (product != null)
                        {
                            int count = 0;
                             var orderDetailQuery = db.Products.Include("OrderDetails").FirstOrDefault(o => o.ProductId == product.ProductId);
                             foreach(OrderDetail order in product.OrderDetails){
                                 if(order.ProductId == product.ProductId){
                                     product.Discontinued = true;
                                     count++;
                                     if(count == 1){
                                         logger.Info($"{product.ProductName} cannot be deleted due to order processing. It has been switched to Discontinued.");
                                     }
                                     db.SaveChanges();
                                 }
                             }
                            
                             if(count == 0){
                             db.Products.Remove(product);
                             db.SaveChanges();
                             logger.Info($"Product (id: {product.ProductId}) deleted");
                             }
                               
                            
                        }

                    }

                } while (choice != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
        public static Category GetCategory(NorthwindConsole_32_KMBContext db)
        {

            // display all blogs
            var categories = db.Categories.OrderBy(b => b.CategoryId);
            Console.WriteLine("Select a Category");
            foreach (Category c in categories)
            {
                Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
            }
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Category Id");
            return null;
        }
        public static Product GetProduct(NorthwindConsole_32_KMBContext db)
        {

            // display all blogs
            var products = db.Products.OrderBy(b => b.ProductId);
            Console.WriteLine("Select a Product to Display");
            foreach (Product p in products)
            {
                Console.WriteLine($"{p.ProductId}: {p.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Category InputCategory(NorthwindConsole_32_KMBContext db)
        {

            Category category = new Category();
            Console.WriteLine("If editing,; do you want to Edit Category Name? y/n");
            var toEdit = Console.ReadLine().ToLower();
            Console.WriteLine("Enter the Category name");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter Product Description");
            category.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName && toEdit == "y"))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Category already exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }

            return category;
        }

        public static Product InputProduct(NorthwindConsole_32_KMBContext db)
        {

            Product product = new Product();
            
            Console.WriteLine("If editing, Do you want to Edit Product Name? y/n");
            var toEdit = Console.ReadLine().ToLower();
            Console.WriteLine("Add/Edit Product name");
            product.ProductName = Console.ReadLine();

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(c => c.ProductName == product.ProductName) && toEdit == "y")
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Product already exists", new string[] { "ProductName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }

            Console.WriteLine("Do you want to continue Add/Edit Product Details?");
            Console.WriteLine("Enter y for yes, n for no");
            var cont = Console.ReadLine().ToLower();
            if (cont == "y"){
            Console.WriteLine("Enter a Supplier ID");
            var suppliers = db.Suppliers.OrderBy(s => s.SupplierId);
            foreach (var s in suppliers){
                Console.WriteLine($"{s.SupplierId} {s.CompanyName}");
            }
            product.SupplierId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Category ID");
            var categories = db.Categories.OrderBy(c => c.CategoryId);
            foreach(var c in categories){
                Console.WriteLine($"{c.CategoryId} {c.CategoryName}");
            }
            product.CategoryId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Quanity per Unit");
            product.QuantityPerUnit = Console.ReadLine();
            Console.WriteLine("Enter the Unit Price");
            product.UnitPrice = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Units in Stock");
            product.UnitsInStock = short.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Reorder Level");
            product.ReorderLevel = short.Parse(Console.ReadLine());
            Console.WriteLine("Is this product discontinued? Enter True for yes, False for no");
            product.Discontinued = bool.Parse(Console.ReadLine());
            
            }

            return product;
        }
    }

}