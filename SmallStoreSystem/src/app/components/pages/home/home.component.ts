import { Component, HostListener, OnInit, Type } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { IStoreData } from '../../../shared/product';
import { AuthentificationService} from '../../../services/authentification.service';
import { ViewportScroller } from '@angular/common';
import { Toast, ToastrService } from 'ngx-toastr';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PopUpComponent } from '../../pop-up/pop-up/pop-up.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  products: IStoreData[] = [];
  filterData: IStoreData[] = [];
  recordCount: number = 0;
  userType: string = '';
  admin: boolean = false;
  backUpData: IStoreData[] = [];
  productTypes: any[] = [];
  productForm!: FormGroup;
  items!: FormArray;
  selectedProductType: any;
  pageYoffset: Number = 0;
  newProduct: boolean = false;
  product: IStoreData = { id: 0, name: '', type: '', price: 0, quantity: 0};
  viewedProductNames: string[] = [];
  editMode = false;

  @HostListener('window:scroll', ['$event']) onScroll(event: any) {
    this.pageYoffset = window.pageYOffset;
  }

  constructor(
    private productService: ProductService,
    private authService: AuthentificationService,
    private fb: FormBuilder,
    private scroll: ViewportScroller,
    private toastr: ToastrService,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {

    this.authService.seeUserInfo();
    if (this.authService.isValidUser) {
      this.userType = this.authService.userType;
      this.admin = this.authService.AdminUser;
      this.InitializeForm();
      this.loadProducts();
      this.retrieveVisited();
    }
    else {
      this.toastr.error('You are not authorized to access this page', 'Error');
    }
  }

  scrollToTop() {
    this.scroll.scrollToPosition([0, 0]);
  }

  InitializeForm() {
    this.productForm = this.fb.group({
      items: this.fb.array([])
    });

  }

  loadProductForm(outliers: IStoreData[]) {
    outliers.forEach((i) => {
      if (i == null) {
        (this.productForm.get('items') as FormArray).push(
          this.fb.group({
            id: '0',
            name: ['', Validators.required],
            type: '',
            price: ['', Validators.required],
            quantity: '',
          })
        );
      } else {
        (this.productForm.get('items') as FormArray).push(
          this.fb.group({
            id: i.id,
            name: i.name,
            type: i.type,
            price: new FormControl(i.price, Validators.required),
            quantity: i.quantity,
          })
        );
      }
    });
  }


  loadProducts() {
    const promise = this.productService.GetAllData().toPromise();
    promise.then((response) => {
      this.filterData = response as any;
      this.retrieveVisited();
      if (this.filterData != null) {
        this.recordCount = this.filterData.length;
        this.backUpData = this.filterData;
        this.productTypes = [...new Set(this.filterData.map(item => item.type))];
        this.products = this.differentiateProductTypes(this.filterData);
        this.loadProductForm(this.products);
      }
        
      else {
        console.log("Error");
        this.toastr.error('Error while loading products', 'Error');
      }
    });
  }

  differentiateProductTypes(allProducts: IStoreData[]): IStoreData[] {
    let emptyRow: IStoreData;
    let previousProductType: string | null = null;
    this.products = [];
    allProducts.forEach(row => {
      if (previousProductType == null) {
        previousProductType = row.type;
      }
      if (row.type != previousProductType) {
        this.products.push(emptyRow);
      }
        this.products.push(row);
        previousProductType = row.type;
      
    });
    return this.products;
  }


  filterByType(productType: string) {
    this.filterData = this.backUpData;

    if (productType !== "0") {
      this.filterData = this.filterData.filter(
        (product) => product.type === productType
      );
    }
    this.recordCount = this.filterData.length;
    this.filterData = this.differentiateProductTypes(this.filterData);
    this.InitializeForm();
    this.loadProductForm(this.filterData);

  }

  sendViewedProduct(id:number) {
    this.productService.productWasVisited(id).subscribe();
  }

  save(id: number, name: string) {
    const currentData = sessionStorage.getItem('viewedProducts');
    const newData = { id: id.toString(), name };
    let isNewDataAdded = false;

    if (currentData) {
      const dataArray = JSON.parse(currentData);
      if (!dataArray.some((item: any) => item.id === newData.id)) {
        dataArray.push(newData);
        isNewDataAdded = true;
      }
      sessionStorage.setItem('viewedProducts', JSON.stringify(dataArray.slice(-3)));
    } else {
      sessionStorage.setItem('viewedProducts', JSON.stringify([newData]));
      isNewDataAdded = true;
    }

    if (isNewDataAdded) {
      const ids = JSON.parse(sessionStorage.getItem('viewedProducts')!).map((item: any) => item.id);
    }
  }

  retrieveVisited() {
    const storedData = sessionStorage.getItem('viewedProducts');
    if (storedData) {
      const viewedProducts = JSON.parse(storedData);
      const viewedProductNames = viewedProducts.map((item: any) => item.name);
      this.viewedProductNames = viewedProductNames;
    }
  }

  deleteProduct(id: number) {
    this.productService.deleteProduct(id).subscribe(
      (response) => {
        if (response === true) {
          this.toastr.success('Product deleted successfully');
          const viewedProducts = JSON.parse(sessionStorage.getItem('viewedProducts') || '[]');
          const index = viewedProducts.findIndex((item: any) => item.id === id.toString());
          if (index > -1) {
            viewedProducts.splice(index, 1);
            sessionStorage.setItem('viewedProducts', JSON.stringify(viewedProducts));
          }
          this.loadProducts();
        }
        else {
          this.toastr.error('Error deleting product');
        }
      },
    );
  }

  addNewProduct() {
    this.newProduct = true;
  }

  cancelAdding() {
    this.newProduct = false;
  }


  onSubmit() {
    this.productService.addProduct(this.product).subscribe((response) => {
      if (response === true) {
        this.loadProducts();
        this.newProduct = false;
        this.toastr.success('Product added successfully');
      }
      else {
        this.toastr.error('Error adding product');

      }
    }
    )
  }

  openDialog(id:number): void {
    this.dialog.open(PopUpComponent, {
      width: '30%',
      data: {id:id}
    })
      .afterClosed().subscribe(val => {
        if (val === 'updated')
        {
          this.loadProducts();
        }
      })

  }
}

