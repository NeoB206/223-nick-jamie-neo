import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { Ledger } from '../../models/ledger.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-ledger',
    templateUrl: './ledger.component.html',
    styleUrl: './ledger.component.css',
    imports: [CommonModule, FormsModule],
    providers: [LedgerService, HttpClient],
})
export class LedgerComponent implements OnInit {
    ledgers: Ledger[] = [];
    fromLedgerId: number | null = null;
    toLedgerId: number | null = null;
    amount: number | null = null;
    transferMessage = '';
    name = '';
    balance = 0;
    deleteId = 0;
    username: string | null = '';

    constructor(private ledgerService: LedgerService) {}

    ngOnInit(): void {
        this.username = this.ledgerService.getLoggedInUsername()
        this.loadLedgers();
    }

    loadLedgers(): void {
        this.ledgerService.getLedgers().subscribe({
            next: (data: Ledger[]) => {
                this.ledgers = data;
            },
            error: (error) => {
                console.error('Error fetching ledgers', error);
            },
        });
    }

    makeTransfer(): void {
        if (this.fromLedgerId !== null && this.toLedgerId !== null && this.amount !== null && this.amount > 0) {
            this.ledgerService.transferFunds(this.fromLedgerId, this.toLedgerId, this.amount).subscribe({
                next: () => {
                    this.transferMessage = 'Transfer successful!';
                    this.loadLedgers();
                },
                error: (error) => {
                    this.transferMessage = `Transfer failed: ${error.error.message}`;
                    console.error('Transfer error', error);
                },
            });
        } else {
            this.transferMessage = 'Please fill in all fields with valid data.';
        }
    }

    onSubmitCreate(): void {
        this.ledgerService.createLedger(this.name, this.balance)?.subscribe(() => window.location.reload());
    }

    onSubmitDelete(): void {
        this.ledgerService.deleteLedger(this.deleteId).subscribe(() => window.location.reload());
    }


    onBalanceInput(event: Event) {
        const input = event.target as HTMLInputElement;
        const value = input.value;
        if (value && value.startsWith('0') && value.length > 1) {
            // Remove leading zeros
            input.value = value.replace(/^0+/, '');
        }
    }

    // Method to handle the delete ID input and remove leading zeros
    onDeleteIdInput(event: Event) {
        const input = event.target as HTMLInputElement;
        const value = input.value;
        if (value && value.startsWith('0') && value.length > 1) {
            // Remove leading zeros
            input.value = value.replace(/^0+/, '');
        }
    }
}
