export class DataPoint {
    Name: number;
    Value: number;

    constructor(data: any) {
        this.Name = data.Name;
        this.Value = data.Value;
    }
}
