import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { IStoreData } from '../shared/product';
import { ToastrModule, ToastrService } from 'ngx-toastr';


@Injectable({
  providedIn: 'root'
})
export class ProductService {

  getProductDataUrl: string = environment.API_Base_Url + '/api/product'
  getProductDataByIdUrl: string = environment.API_Base_Url + '/api/productById?id='
  postVisitStatusUrl: string = environment.API_Base_Url + '/api/visitStatus?id='
  deleteProductUrl: string = environment.API_Base_Url + '/api/deleteProduct?id='
  addNewProductUrl: string = environment.API_Base_Url + '/api/addProduct'
  getProductNamesUrl: string = environment.API_Base_Url + '/api/getProductNames'
  updateProductUrl: string = environment.API_Base_Url + '/api/updateProduct?id='
  private products: IStoreData[] = [];

  constructor(private http: HttpClient,
  ) { }

  GetAllData(): Observable<IStoreData[]> {
    return this.http.get<IStoreData[]>(this.getProductDataUrl).pipe(
      map((products) =>
        this.products = products
      ),
      tap(data => console.log('Record count:' + data.length)),
    );
  }
  getProductById(productId:number): Observable<IStoreData[]> {
    return this.http.get<IStoreData[]>(this.getProductDataByIdUrl + productId).pipe(
      map((products) =>
        this.products = products
      ),
    );
  }

  productWasVisited(productId: number) {
    console.log(productId);
    return this.http.post(this.postVisitStatusUrl + productId, null)
  }

  deleteProduct(productId: number) {
    return this.http.delete(this.deleteProductUrl + productId);
  }

  addProduct(product: IStoreData) {
    return this.http.post(this.addNewProductUrl, product);
  }
  getProductNames(productIds: string[]): Observable<string[]> {
    const params = { ids: productIds.join(',') };
    console.log(productIds);
    return this.http.get<string[]>(this.getProductNamesUrl, { params });
  }

  updateProduct(productData: any, productId: number): Observable<any> {
    console.log(productData);
    return this.http.put(this.updateProductUrl + productId, productData);
  }
}
