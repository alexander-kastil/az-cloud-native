namespace DaprBankActor;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using FoodApp;
using IBankActorInterface;

public class BankActor : Actor, IBankActor, IRemindable
{

    private AILogger logger;
    private readonly BankService bank;
    private string AccountId;

    public BankActor(ActorHost host, BankService bank, AILogger ai)
        : base(host)
    {
        this.bank = bank;
        this.AccountId = this.Id.GetId();
        logger = ai;
    }

    public async Task<AccountBalance> SetupNewAccount(decimal startingDeposit) 
    {
        logger.LogEvent("SetupNewAccount", new { AccountId = AccountId, startingDeposit = startingDeposit });
        var starting = new AccountBalance()
        {
            AccountId = AccountId,
            Balance = startingDeposit, 
        };

        var balance = await this.StateManager.GetOrAddStateAsync<AccountBalance>(AccountId, starting);
        return balance;
    }
   
    public async Task<AccountBalance> GetAccountBalance()
    {
        return await this.StateManager.GetStateAsync<AccountBalance>(AccountId);
    }

    public async Task<TransactionResponse> Deposit(DepositRequest deposit)
    {
        logger.LogEvent("Deposit", new { AccountId = AccountId, deposit = deposit.Amount });
        var balance = await this.StateManager.GetStateAsync<AccountBalance>(AccountId);
        var updated = this.bank.Deposit(balance.Balance, deposit.Amount);
        balance.Balance = updated;
        await StateManager.SetStateAsync(AccountId, balance);
        return new TransactionResponse(){Status = "Success", Message = $"Deposited {deposit.Amount}"};
    }
    public async Task<TransactionResponse> Withdraw(WithdrawRequest withdraw)
    {
        logger.LogEvent("Withdraw", new { AccountId = AccountId, withdraw = withdraw.Amount });
        var response = new TransactionResponse(){Status = "Success", Message = $"Withdrew {withdraw.Amount}"};
        var balance = await this.StateManager.GetStateAsync<AccountBalance>(AccountId);
        if(balance.Balance < withdraw.Amount)
        {
            response.Status = "Failure";
            response.Message = $"Insufficient funds to withdraw {withdraw.Amount}";
            return response;
        }
        var updated = this.bank.Withdraw(balance.Balance, withdraw.Amount);
        balance.Balance = updated;
        await this.StateManager.SetStateAsync(AccountId, balance);
        return response;
    }

    public Task UnRegisterReoccurring(TransferType type) 
    {
        return type switch 
        {
            TransferType.Deposit => this.UnregisterReminderAsync("deposit"),
            TransferType.Withdraw => this.UnregisterReminderAsync("withdraw"),
            _ => Task.CompletedTask
        };
    }

    public Task RegisterReoccurring(TransferType type, decimal amount)
    {
        var serializedParams = JsonSerializer.SerializeToUtf8Bytes(amount);

        return type switch 
        {
            TransferType.Deposit => this.RegisterReminderAsync("deposit", serializedParams, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)),
            TransferType.Withdraw => this.RegisterReminderAsync("withdraw", serializedParams, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)),
            _ => Task.CompletedTask,
        };
    }

    public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        var request = JsonSerializer.Deserialize<decimal>(state);

        return reminderName switch
        {
            "withdraw" => this.Withdraw(new WithdrawRequest(){ Amount = request}),
            "deposit" => this.Deposit(new DepositRequest(){ Amount = request}),
            _ => Task.CompletedTask 
        };
    }

    protected override Task OnActivateAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task OnDeactivateAsync()
    {
        return Task.CompletedTask;
    }
}

