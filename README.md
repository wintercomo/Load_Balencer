# Load_Balencer
A basic Load Balencer

Start a localhost server with the BasicHttpServer.EXE

LoadBalencerClassLibrary contains all functionality for the loadbalencer + some algorithms

LoadBalancerAlgorithmsCL Contains a few algorithms to show that reflection works.

build the CL class library and drag the .DLL file to the debug folder within the Loadbalancer folder

By pressing the button on the loadbalencer "Load algorithms" all DLL files are scanned in the bin folder. If a dll file contains classes that implement the IALgorithm interface, they are added to the list of algorithms.