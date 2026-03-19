using System.Reflection;
using Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;

namespace Unit.Pipeline.Cluster.CircBreaker;

public class CircuitBreakerTests
{
    private object GetHostState(CircuitBreaker cb, int idx)
    {
        var field = cb
            .GetType()
            .GetField("_hostsStates", BindingFlags.NonPublic | BindingFlags.Instance);
        
        var array = (Array)field!.GetValue(cb)!;
        return array.GetValue(idx)!;
    }
    
    private T GetFieldValue<T>(object obj, string fieldName)
    {
        var field = obj
            .GetType()
            .GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        return (T)field!.GetValue(obj)!;
    }
    
    private void SetOpenedAtTicks(CircuitBreaker cb, int hostIdx, long value)
    {
        var hostState = GetHostState(cb, hostIdx);
        var field = hostState
            .GetType()
            .GetField("OpenedAtTicks", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        field!.SetValue(hostState, value);
    }
    
    [Fact]
    public void CanExecute_WhenCircuitIsOpen_ReturnsFalse()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(2, 1, 5);
        
        // Act
        circuitBreaker.ReportResult(1, false);
        var result = circuitBreaker.CanExecute(1);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void CanExecute_WhenCircuitIsClosed_ReturnsTrue()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(2, 1, 5);
        
        // Act
        var result = circuitBreaker.CanExecute(1);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void CanExecute_AfterTimeout_ReturnsTrue()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(1, 1, 1);
        circuitBreaker.ReportResult(0, false);
        SetOpenedAtTicks(circuitBreaker, 0, Environment.TickCount64 - 1100); // подменяем время
        
        // Act
        var result = circuitBreaker.CanExecute(0);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenMultipleFailures_OpensCircuit()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(1, 3, 5);
        
        // Act
        circuitBreaker.ReportResult(0, false);
        var result1 = circuitBreaker.CanExecute(0);
        
        circuitBreaker.ReportResult(0, false);
        var result2 = circuitBreaker.CanExecute(0);
        
        circuitBreaker.ReportResult(0, false);
        var result3 = circuitBreaker.CanExecute(0);
        
        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.False(result3);
    }

    [Fact]
    public void CanExecute_WhenMultipleHosts_HostsAreIndependent()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(2, 3, 5);
        
        // Act
        circuitBreaker.ReportResult(0, false);
        circuitBreaker.ReportResult(0, false);
        circuitBreaker.ReportResult(0, false);
        
        var host1 = circuitBreaker.CanExecute(0);
        var host2 = circuitBreaker.CanExecute(1);
        
        // Assert
        Assert.False(host1);
        Assert.True(host2);
    }
    
    [Fact]
    public void ReportResult_AfterFailure_HostStateIsOpen()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(1, 1, 5);
        
        // Act
        circuitBreaker.ReportResult(0, false);
        var hostState = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "State");
        
        // Assert
        Assert.Equal(1, hostState);
    }
    
    [Fact]
    public void CanExecute_AfterTimeOut_HostStateIsHalfOpen()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(1, 1, 1);
        circuitBreaker.ReportResult(0, false);
        SetOpenedAtTicks(circuitBreaker, 0, Environment.TickCount64 - 1100); // подменяем время
        circuitBreaker.CanExecute(0);
        
        // Act
        var hostState = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "State");
        var hostFailures = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "Failures");
        
        // Assert
        Assert.Equal(2, hostState);
        Assert.Equal(1, hostFailures);
    }
    
    [Fact]
    public void ReportResult_AfterSuccess_HostStateIsClosed()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(1, 1, 1);
        circuitBreaker.ReportResult(0, false);
        SetOpenedAtTicks(circuitBreaker, 0, Environment.TickCount64 - 1100); // подменяем время
        
        // Act
        circuitBreaker.CanExecute(0);
        var hostState1 = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "State");
        circuitBreaker.ReportResult(0, true);
        var hostState2 = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "State");
        var hostFailures = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "Failures");
        
        // Assert
        Assert.Equal(2, hostState1);
        Assert.Equal(0, hostState2);
        Assert.Equal(0, hostFailures);
    }
    
    [Fact]
    public void ReportResult_AfterFailure_FailuresCountIsValid()
    {
        // Arrange
        var circuitBreaker = new CircuitBreaker(1, 1, 1);
        
        // Act
        var hostFailures1 = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "Failures");
        circuitBreaker.ReportResult(0, false);
        var hostFailures2 = GetFieldValue<int>(GetHostState(circuitBreaker, 0), "Failures");
        
        // Assert
        Assert.Equal(0, hostFailures1);
        Assert.Equal(1, hostFailures2);
    }
}