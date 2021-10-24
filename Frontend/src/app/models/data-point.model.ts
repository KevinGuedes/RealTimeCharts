export class DataPoint {
    name: number;
    value: number;

    constructor(data: any) {
        this.name = data.name;
        this.value = data.value;
    }
}
