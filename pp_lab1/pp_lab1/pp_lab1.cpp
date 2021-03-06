#include "pch.h"
#include <iostream>

using namespace std;

const double MIN_DOUBLE = 0.1;
const int ARGS_COUNT = 3;

struct Matrix
{
	int row, column, size, b, e;
	vector<vector<double>>* copy;
	istream* in;
};

DWORD WINAPI ReadMatrix(CONST LPVOID data)
{
	auto matrix = (Matrix*)data;
	double size;
	vector<double> row;

	for (size_t i = 0; i < matrix->size; i++)
	{
		row.clear();

		for (size_t j = 0; j < matrix->size; j++)
		{
			(*(matrix->in)) >> size;
			row.push_back(size);
		}

		(*(matrix->copy)).push_back(row);
	}

	DWORD dwResult = 0;
	return dwResult;
}

DWORD WINAPI FindMinorsMatrix(CONST LPVOID data)
{
	auto matrix = (Matrix*)data;

	for (size_t i = matrix->b; i < matrix->e; ++i)
	{
		if (i != matrix->row && !(abs((*(matrix->copy))[i][matrix->column]) < MIN_DOUBLE))
		{
			for (size_t j = matrix->column + 1; j < (*(matrix->copy)).size(); ++j)
			{
				(*(matrix->copy))[i][j] -= (*(matrix->copy))[matrix->row][j] * (*(matrix->copy))[i][matrix->column];
			}
		}
	}

	ExitThread(0);
	DWORD dwResult = 0;
	return dwResult;
}

void InitializeMatrix(istream& input, double& threadNumber, vector<vector<double>>& inputData, int& size)
{
	input >> size;
	threadNumber = (threadNumber > size) ? size : threadNumber;

	auto* matrix = new Matrix;
	matrix->in = &input;
	matrix->copy = &inputData;
	matrix->size = size;

	auto data = (LPVOID)matrix;
	HANDLE* handle = new HANDLE;
	*handle = CreateThread(NULL, 0, &ReadMatrix, data, 0, NULL);
	WaitForMultipleObjects(1, handle, true, INFINITE);
}

int CalculateRang(vector<vector<double>> inputData, int& size, double threadNumber)
{
	vector<vector<double>> matrixCopy(inputData);
	int result = size;
	vector<bool> processed(size);

	for (size_t i = 0; i < size; i++)
	{
		size_t j;

		for (j = 0; j < size; j++)
		{
			double number = matrixCopy[j][i];

			if (!processed[j] && !(abs(number) < MIN_DOUBLE))
			{
				break;
			}
		}

		if (j == size)
		{
			--result;
			continue;
		}

		processed[j] = true;
		for (size_t k = i + 1; k < size; k++)
		{
			matrixCopy[j][k] = matrixCopy[j][k] / matrixCopy[j][i];
		}

		int step = size / threadNumber;
		int threadsAmount = threadNumber;
		HANDLE* handles = new HANDLE[threadNumber];

		for (size_t l = 0; threadsAmount != 0; l += step)
		{
			--threadsAmount;
			auto* matrix = new Matrix;
			matrix->copy = &matrixCopy;
			matrix->column = i;
			matrix->row = j;
			matrix->b = l;

			matrix->e = (threadsAmount != 0) ? (l + step) : size;
			auto lpVoidMatrix = (LPVOID)matrix;
			handles[threadsAmount] = CreateThread(NULL, 0, &FindMinorsMatrix, lpVoidMatrix, 0, NULL);
		}

		WaitForMultipleObjects(threadNumber, handles, true, INFINITE);
	}

	return result;
}

int main(int argc, char* argv[])
{
	HANDLE process = GetCurrentProcess();
	SetProcessAffinityMask(process, 0b1);
	double start = clock();

	if (argc != ARGS_COUNT)
	{
		cout << "Incorrect params count" << endl;
		return 1;
	}

	ifstream input;
	input.open(argv[1]);
	if (!input)
	{
		cout << "File not found" << endl;
		return 1;
	}

	auto threadNumber = stod(argv[2]);
	vector<vector<double>> inputMatrix;
	int matrixSize = 0;

	InitializeMatrix(input, threadNumber, inputMatrix, matrixSize);
	cout << "Rang: " << CalculateRang(inputMatrix, matrixSize, threadNumber) << endl;

	double end = clock();
	cout << "time: " << end - start << endl;

	return 0;
}