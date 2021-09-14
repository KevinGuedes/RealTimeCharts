import { DataGenerationRate } from "src/app/models/data-generation-rate.enum";
import { DataType } from "src/app/models/data-type.enum";

export class GenerateDataRequest {
    Rate: DataGenerationRate;
    DataType: DataType;
    ConnectionId: string;

    constructor(rate: DataGenerationRate, dataType: DataType, connectionId: string) {
        this.Rate = rate;
        this.DataType = dataType;
        this.ConnectionId = connectionId;
    }
}
