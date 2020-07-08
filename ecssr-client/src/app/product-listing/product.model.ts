export interface IProduct {
    id: number;
    name: string;
    pictures: IProductPicture[];
    category: string;
    color: string;
    companyName: string;
    description: string;
    model: string;
    price: number;
    videoUrl: string;
}

export interface IProductPicture {
    id: number;
    imageUrl: string;
}

export interface IReport {
    totalIndexed: number;
    totalProducts: number;
    totalPictures: number;
}