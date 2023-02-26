import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { IStoreData } from '../../../shared/product';

@Component({
  selector: 'app-product-page',
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.css']
})
export class ProductPageComponent implements OnInit {

  product: IStoreData[] = [];

  constructor(activatedRoute: ActivatedRoute, productService: ProductService) { //Not yet used
    activatedRoute.params.subscribe((params) => {
      if (params['id']) {
        console.log("");
        productService.getProductById(params['id']).subscribe(
          (result) => {
            this.product = result;
          },
          (error) => {
            console.error(error);
          }
        );
      }
    });
  }

  ngOnInit(): void {
  }
}
