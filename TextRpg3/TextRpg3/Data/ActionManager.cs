using TextRpg3.Data.Models;
using TextRpg3.UI;

namespace TextRpg3.Data;

public class ActionManager
{
    private const int PointsToPatrol = 5;
    private const int PointsToTraining = 15;
    private const int PointsToAdventure = 10;


    private readonly UIManager _uiManager;
    private Player _player;

    public ActionManager(UIManager uiManager, Player player)
    {
        _uiManager = uiManager;
        _player = player;
    }
    public void ActionSequence(Action action)
    {
        switch (action)
        {
            case Action.Patrol:
                ActionPatrol();
                break;
            case Action.Training:
                ActionTraining();
                break;
            case Action.Adventure:
                ActionAdventure();
                break;
            default:
                break;
        }
    }

    private bool UseStamina(int needPoints)
    {
        if (_player.Stamina < needPoints)
        {
            _uiManager.DisplayInsufficientStamina();
            return false;
        }

        _player.Stamina -= needPoints;
        return true;
    }
    private void ActionPatrol()
    {
        if (!UseStamina(PointsToPatrol)) return;
        int patrolResult = new Random().Next(0, 10);
        if (patrolResult < 1)
        {
            _player.AddGold(-500);
        }else if(patrolResult < 2)
        {
            _player.AddGold(2000);
        }else if (patrolResult < 4)
        {
            _player.AddGold(1000);
        }else if (patrolResult < 7)
        {
            _player.AddGold(500);
        }

        _uiManager.DisplayPatrolResult(patrolResult);

    }
    private void ActionTraining()
    {
        if (!UseStamina(PointsToTraining)) return;
        int trainingResult = new Random().Next(0, 100);
        if (trainingResult < 15)
        {
            _player.AddExp(60);
        }else if(trainingResult < 60)
        {
            _player.AddExp(40);
        }else if (trainingResult < 100)
        {
            _player.AddExp(30);
        }


        _uiManager.DisplayTrainingResult(trainingResult);
    }
    private void ActionAdventure()
    {

        if (!UseStamina(PointsToAdventure)) return;

        bool isSuccess = new Random().Next(0, 2) == 0;
        if(isSuccess)_player.AddGold(500);

        _uiManager.DisplayAdventureResult(isSuccess);
    }
}