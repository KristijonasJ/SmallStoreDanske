import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ProductService } from '../../../services/product.service';
import { IStoreData } from '../../../shared/product';
import { HomeComponent } from '../../pages/home/home.component';
import { MatDialogRef } from '@angular/material/dialog';


@Component({
  selector: 'app-pop-up',
  templateUrl: './pop-up.component.html',
  styleUrls: ['./pop-up.component.css']
})
export class PopUpComponent implements OnInit{

  product: IStoreData = { id: 0, name: '', type: '', price: 0, quantity: 0};
  productForm!: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private productService: ProductService,
    private toastr: ToastrService,
    private dialogRef : MatDialogRef<PopUpComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) { }

  ngOnInit(): void{
    this.productForm = this.formBuilder.group({
      id: 0,
      type: ['', Validators.required],
      name: ['', Validators.required],
      price: ['', Validators.required],
      quantity: ['', Validators.required]
      })
  }

  editProduct() {
    if (this.productForm.value.name && this.productForm.value.type && this.productForm.value.price && this.productForm.value.type) {
      this.productService.updateProduct(this.productForm.value, this.data.id).subscribe(
        (response: any) => {
          if (response === true) {
            this.toastr.success('Product updated successfully', 'Success');
            this.productForm.reset();
            this.dialogRef.close('updated');
          }
          else {
            this.toastr.error(response);
          }
        },
        (error: any) => {
          console.log(error);
        })
    }
    else {
      this.toastr.error('Please fill all the fields', 'Error');
    }
    
  }
}
