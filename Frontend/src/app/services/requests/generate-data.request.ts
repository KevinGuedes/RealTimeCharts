import { DataGenerationRate } from "src/app/models/data-generation-rate.enum";
import { DataType } from "src/app/models/data-type.enum";

export class GenerateDataRequest {
    dataGenerationRate: DataGenerationRate;
    dataType: DataType;
    connectionId: string;

    constructor(rate: DataGenerationRate, dataType: DataType, connectionId: string) {
        this.dataGenerationRate = rate;
        this.dataType = dataType;
        this.connectionId = connectionId;
    }
}
