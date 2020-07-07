export interface IProduct {
    id: number;
    name: string;
    pictures: IProductPicture[];
}

export interface IProductPicture {
    id: number;
    imageUrl: string;
}